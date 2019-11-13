namespace SharpDeck.Connectivity
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Events;
    using SharpDeck.Events.Received;

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
        /// Initializes a new instance of the <see cref="StreamDeckActionProvider" /> class.
        /// </summary>
        /// <param name="connection">The connection responsible for invoking actions received by the Stream Deck.</param>
        /// <param name="client">The Stream Deck client.</param>
        public StreamDeckActionProvider(IStreamDeckActionEventPropogator connection, IStreamDeckClient client)
        {
            this.Cache = new StreamDeckActionCache(client);
            this.Client = client;

            // responsible for caching
            connection.WillAppear += this.Action_WillAppear;

            // action propagation
            connection.DidReceiveSettings               += (_, e) => this.PropagateOnAction(e, a => a.OnDidReceiveSettings);
            connection.KeyDown                          += (_, e) => this.PropagateOnAction(e, a => a.OnKeyDown);
            connection.KeyUp                            += (_, e) => this.PropagateOnAction(e, a => a.OnKeyUp);
            connection.PropertyInspectorDidAppear       += (_, e) => this.PropagateOnAction(e, a => a.OnPropertyInspectorDidAppear);
            connection.PropertyInspectorDidDisappear    += (_, e) => this.PropagateOnAction(e, a => a.OnPropertyInspectorDidDisappear);
            connection.SendToPlugin                     += (_, e) => this.PropagateOnAction(e, a => a.OnSendToPlugin);
            connection.TitleParametersDidChange         += (_, e) => this.PropagateOnAction(e, a => a.OnTitleParametersDidChange);
            connection.WillDisappear                    += (_, e) => this.PropagateOnAction(e, a => a.OnWillDisappear);
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
        private void Action_WillAppear(object sender, ActionEventArgs<AppearancePayload> e)
        {
            // check if the action type is handled by this instance
            if (!this.ActionFactory.TryGetValue(e.Action, out var valueFactory))
            {
                return;
            }

            try
            {
                _syncRoot.Wait();

                if (!this.Cache.TryGet(e, out var action, e.Payload))
                {
                    action = valueFactory(e);
                    this.Cache.Add(e, action);

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
        /// Attempts to propagate the event on an associated action, if it exists within the cache.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments parameters.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <param name="getPropagator">The selector to get the propagation event.</param>
        private void PropagateOnAction<T>(T args, Func<StreamDeckActionEventPropagator, Func<T, Task>> getPropagator)
            where T : IActionEventArgs
        {
            try
            {
                _syncRoot.Wait();
                if (this.Cache.TryGet(args, out var action))
                {
                    // invoke the propagation
                    getPropagator(action)(args);
                }
            }
            finally
            {
                _syncRoot.Release();
            }
        }
    }
}
