namespace SharpDeck.Connectivity
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Events;
    using SharpDeck.Events.Received;
    using SharpDeck.Threading;

    /// <summary>
    /// Provides connectivity between a <see cref="StreamDeckClient"/> and registered <see cref="StreamDeckAction"/>.
    /// </summary>
    public class StreamDeckActionProvider
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionProvider"/> class.
        /// </summary>
        /// <param name="client">The parent Stream Deck client.</param>
        public StreamDeckActionProvider(IStreamDeckClient client)
        {
            this.Cache = new StreamDeckActionCache(client);
            this.Client = client;

            // responsible for caching
            client.WillAppear += this.Action_WillAppear;

            // general propagation
            client.DidReceiveSettings += (_, e) => this.TryPropagate(e, a => a.OnDidReceiveSettings);
            client.KeyDown += (_, e) => this.TryPropagate(e, a => a.OnKeyDown);
            client.KeyUp += (_, e) => this.TryPropagate(e, a => a.OnKeyUp);
            client.PropertyInspectorDidAppear += (_, e) => this.TryPropagate(e, a => a.OnPropertyInspectorDidAppear);
            client.PropertyInspectorDidDisappear += (_, e) => this.TryPropagate(e, a => a.OnPropertyInspectorDidDisappear);
            client.SendToPlugin += (_, e) => this.TryPropagate(e, a => a.OnSendToPlugin);
            client.TitleParametersDidChange += (_, e) => this.TryPropagate(e, a => a.OnTitleParametersDidChange);
            client.WillDisappear += (_, e) => this.TryPropagate(e, a => a.OnWillDisappear);
        }

        /// <summary>
        /// Gets the actions factory, used to initialize new instances of actions.
        /// </summary>
        private IDictionary<string, Func<ActionEventArgs<AppearancePayload>, StreamDeckAction>> ActionFactory { get; } = new Dictionary<string, Func<ActionEventArgs<AppearancePayload>, StreamDeckAction>>();

        /// <summary>
        /// Gets the actions that have been initialized, and can be invoked when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private IStreamDeckActionCache Cache { get; }

        /// <summary>
        /// Gets the client.
        /// </summary>
        private IStreamDeckClient Client { get; }

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
        /// Handles the <see cref="StreamDeckActionEventPropagator.WillAppear"/> event of the <see cref="Client"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private async void Action_WillAppear(object sender, ActionEventArgs<AppearancePayload> e)
        {
            // check if the action type is handled by this instance
            if (!this.ActionFactory.TryGetValue(e.Action, out var valueFactory))
            {
                return;
            }

            try
            {
                await _syncRoot.WaitAsync();

                if (!this.Cache.TryGet(e, out var action, e.Payload))
                {
                    action = valueFactory(e);
                    await this.Cache.AddAsync(e, action);

                    action.Initialize(e, this.Client);
                }

                _ = action.OnWillAppear(e);
            }
            finally
            {
                _syncRoot.Release();
            }
        }

        /// <summary>
        /// Attempts to propagate the event to an associated action, if it exists within the cache.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments parameters.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <param name="getPropagator">The selector to get the propagation event.</param>
        private void TryPropagate<T>(T args, Func<StreamDeckActionEventPropagator, Func<T, Task>> getPropagator)
            where T : IActionEventArgs
        {
            try
            {
                _syncRoot.Wait();
                if (this.Cache.TryGet(args, out var action))
                {
                    // invoke the propagation
                    using (SynchronizationContextSwitcher.NoContext())
                    {
                        getPropagator(action)(args);
                    }
                }
            }
            finally
            {
                _syncRoot.Release();
            }
        }
    }
}
