namespace SharpDeck.Connectivity
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Events;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides connectivity between a <see cref="StreamDeckClient"/> and registered <see cref="StreamDeckAction"/>.
    /// </summary>
    public sealed class StreamDeckActionProvider : IDisposable
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionProvider" /> class.
        /// </summary>
        /// <param name="connection">The connection responsible for invoking actions received by the Stream Deck.</param>
        /// <param name="client">The Stream Deck client.</param>
        public StreamDeckActionProvider(IStreamDeckActionEventPropagator connection, IStreamDeckClient client)
        {
            this.Cache = new StreamDeckActionCacheCollection(client);
            this.Client = client;

            // responsible for caching
            connection.WillAppear += this.Action_WillAppear;

            // action propagation
            connection.DidReceiveSettings               += (_, e) => this.InvokeOnAction(e, a => a.OnDidReceiveSettings);
            connection.KeyDown                          += (_, e) => this.InvokeOnAction(e, a => a.OnKeyDown);
            connection.KeyUp                            += (_, e) => this.InvokeOnAction(e, a => a.OnKeyUp);
            connection.PropertyInspectorDidAppear       += (_, e) => this.InvokeOnAction(e, a => a.OnPropertyInspectorDidAppear);
            connection.PropertyInspectorDidDisappear    += (_, e) => this.InvokeOnAction(e, a => a.OnPropertyInspectorDidDisappear);
            connection.SendToPlugin                     += (_, e) => this.InvokeOnAction(e, a => a.OnSendToPlugin);
            connection.TitleParametersDidChange         += (_, e) => this.InvokeOnAction(e, a => a.OnTitleParametersDidChange);
            connection.WillDisappear                    += (_, e) => this.InvokeOnAction(e, a => a.OnWillDisappear);
        }

        /// <summary>
        /// Gets the actions factory, used to initialize new instances of actions.
        /// </summary>
        private IDictionary<string, Func<ActionEventArgs<AppearancePayload>, StreamDeckAction>> ActionFactory { get; } = new ConcurrentDictionary<string, Func<ActionEventArgs<AppearancePayload>, StreamDeckAction>>();

        /// <summary>
        /// Gets the actions that have been initialized, and can be invoked when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private IStreamDeckActionCacheCollection Cache { get; }

        /// <summary>
        /// Gets the client.
        /// </summary>
        private IStreamDeckClient Client { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.ActionFactory?.Clear();
            this.Cache?.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Registers a new <see cref="StreamDeckAction"/> for the specified action UUID.
        /// </summary>
        /// <typeparam name="T">The type of Stream Deck action.</typeparam>
        /// <param name="action">The action UUID associated with the Stream Deck.</param>
        /// <param name="valueFactory">The value factory, used to initialize a new action.</param>
        public void Register<T>(string action, Func<ActionEventArgs<AppearancePayload>, T> valueFactory)
            where T : StreamDeckAction
            => this.ActionFactory.Add(action, valueFactory);

        /// <summary>
        /// Handles the <see cref="IStreamDeckActionEventPropagator.WillAppear"/> event of the <see cref="Client"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private void Action_WillAppear(object sender, ActionEventArgs<AppearancePayload> args)
        {
            // check if the action type is handled by this instance
            if (this.ActionFactory.TryGetValue(args.Action, out var valueFactory))
            {
                _ = this.Action_WillAppearAsync(args, valueFactory);
            }
        }

        /// <summary>
        /// Handles initializing the action and invoking the <see cref="IStreamDeckActionEventPropagator.WillAppear"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        /// <param name="valueFactory">The value factory used to construct a new instance of the action when required.</param>
        private async Task Action_WillAppearAsync(ActionEventArgs<AppearancePayload> args, Func<ActionEventArgs<AppearancePayload>, StreamDeckAction> valueFactory)
        {
            try
            {
                await _syncRoot.WaitAsync();

                if (!this.Cache.TryGet(args, out var action, args.Payload))
                {
                    action = valueFactory(args);
                    await this.Cache.AddAsync(args, action);

                    action.Initialize(args, this.Client);
                }

                _ = this.InvokeOnActionAsync(action, args, a => a.OnWillAppear);
            }
            catch (Exception ex)
            {
                await this.Client.ShowAlertAsync(args.Context);
                await this.Client.LogMessageAsync(ex.Message);
            }
            finally
            {
                _syncRoot.Release();
            }
        }

        /// <summary>
        /// Attempts to invoke the event on its associated instance, using <paramref name="getPropagator"/> to determine the delegate that should be invoked.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments parameters.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <param name="getPropagator">The selector to get the propagation event.</param>
        private void InvokeOnAction<T>(T args, Func<StreamDeckActionEventPropagator, Func<T, Task>> getPropagator)
            where T : IActionEventArgs
        {
            // invokes the event on the action
            if (this.Cache.TryGet(args, out var action))
            {
                _ = this.InvokeOnActionAsync(action, args, getPropagator);
            }
        }

        /// <summary>
        /// Attempts to propagate the event on an associated action, if it exists within the cache.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments parameters.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="getPropagator">The selector to get the propagation event.</param>
        /// <returns>The task of invoking the event on the action.</returns>
        private Task InvokeOnActionAsync<T>(StreamDeckAction action, T args, Func<StreamDeckActionEventPropagator, Func<T, Task>> getPropagator)
            where T : IActionEventArgs
        {
            return Task.Run(async () =>
            {
                try
                {
                    // we dont want the main thread to await the invocation, but we do want to maintain a context to enable error capturing
                    await getPropagator(action)(args);
                }
                catch (Exception ex)
                {
                    await this.Client.LogMessageAsync(ex.Message);
                    await this.Client.ShowAlertAsync(action.Context);
                }
            });
        }
    }
}
