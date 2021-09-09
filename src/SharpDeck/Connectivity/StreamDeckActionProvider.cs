namespace SharpDeck.Connectivity
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides connectivity between a <see cref="IStreamDeckConnection"/> and registered <see cref="StreamDeckAction"/>.
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
        /// <param name="connection">The connection with the Stream Deck responsible for sending and receiving events and messages.</param>
        public StreamDeckActionProvider(IStreamDeckConnection connection)
        {
            this.Cache = new StreamDeckActionCacheCollection(connection);
            this.Connection = connection;

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
        /// Gets the registered actions, used to initialize new instances of actions.
        /// </summary>
        private IDictionary<string, Func<StreamDeckAction>> RegisteredActions { get; } = new ConcurrentDictionary<string, Func<StreamDeckAction>>();

        /// <summary>
        /// Gets the actions that have been initialized, and can be invoked when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private IStreamDeckActionCacheCollection Cache { get; }

        /// <summary>
        /// Gets the connection with the Stream Deck responsible for sending and receiving events and messages.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.RegisteredActions?.Clear();
            this.Cache?.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Registers a new <see cref="StreamDeckAction"/> for the specified action UUID.
        /// </summary>
        /// <typeparam name="T">The type of Stream Deck action.</typeparam>
        /// <param name="actionUUID">The action UUID associated with the Stream Deck.</param>
        /// <param name="valueFactory">The value factory, used to initialize a new action.</param>
        public void Register<T>(string actionUUID, Func<T> valueFactory)
            where T : StreamDeckAction
            => this.RegisteredActions.Add(actionUUID, valueFactory);

        /// <summary>
        /// Attempts to get the action instance for the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">The <see cref="IActionEventArgs"/> instance containing the event data.</param>
        /// <param name="value">The <see cref="StreamDeckAction"/> instanced associated with the <paramref name="args"/>.</param>
        /// <returns><c>true</c> when an instance exists for the given <paramref name="args"/>; otherwise <c>false</c>.</returns>
        public bool TryGet(IActionEventArgs args, out StreamDeckAction value)
            => this.Cache.TryGet(args, out value);

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillAppear"/> event of the <see cref="Connection"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private void Action_WillAppear(object sender, ActionEventArgs<AppearancePayload> args)
        {
            // Check if the action type is handled by this instance.
            if (this.RegisteredActions.TryGetValue(args.Action, out var valueFactory))
            {
                _ = this.Action_WillAppearAsync(args, valueFactory);
            }
        }

        /// <summary>
        /// Handles initializing the action and invoking the <see cref="IStreamDeckConnection.WillAppear"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        /// <param name="valueFactory">The value factory used to construct a new instance of the action when required.</param>
        private async Task Action_WillAppearAsync(ActionEventArgs<AppearancePayload> args, Func<StreamDeckAction> valueFactory)
        {
            try
            {
                await _syncRoot.WaitAsync();

                if (!this.Cache.TryGet(args, out var action, args.Payload))
                {
                    action = valueFactory();
                    await this.Cache.AddAsync(args, action);

                    action.Initialize(args, this.Connection);
                }

                _ = this.InvokeOnActionAsync(action, args, a => a.OnWillAppear);
            }
            catch (Exception ex)
            {
                await this.Connection.ShowAlertAsync(args.Context);
                await this.Connection.LogMessageAsync(ex.Message);
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
        private void InvokeOnAction<T>(T args, Func<StreamDeckAction, Func<T, Task>> getPropagator)
            where T : IActionEventArgs
        {
            // Invokes the event on the action.
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
        private Task InvokeOnActionAsync<T>(StreamDeckAction action, T args, Func<StreamDeckAction, Func<T, Task>> getPropagator)
            where T : IActionEventArgs
        {
            return Task.Run(async () =>
            {
                try
                {
                    // We dont want the main thread to await the invocation, but we do want to maintain a context to enable error capturing.
                    await getPropagator(action)(args);
                }
                catch (Exception ex)
                {
                    await this.Connection.LogMessageAsync(ex.Message);
                    await this.Connection.ShowAlertAsync(action.Context);
                }
            });
        }
    }
}
