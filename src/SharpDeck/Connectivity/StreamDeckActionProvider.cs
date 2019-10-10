namespace SharpDeck.Connectivity
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Events.Received;
    using SharpDeck.Extensions;

    /// <summary>
    /// Provides connectivity between a <see cref="StreamDeckClient"/> and registered <see cref="StreamDeckAction"/>.
    /// </summary>
    public class StreamDeckActionProvider
    {
        /// <summary>
        /// The key used to assigned a SharpDeck UUID to <see cref="SettingsPayload.Settings"/>.
        /// </summary>
        private const string SHARP_DECK_UUID_KEY = "__sharpDeckUUID";

        /// <summary>
        /// The synchronization root
        /// </summary>
        private readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionProvider"/> class.
        /// </summary>
        /// <param name="client">The parent Stream Deck client.</param>
        public StreamDeckActionProvider(StreamDeckClient client)
        {
            this.Client = client;

            // responsible for caching
            client.WillAppear += this.Action_WillAppear;

            // general propgation
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
        /// Gets the actions that have been initialized, and can be invoked when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private Dictionary<string, StreamDeckActionCacheEntry> ActionCache { get; } = new Dictionary<string, StreamDeckActionCacheEntry>();

        /// <summary>
        /// Gets the actions factory, used to initialize new instances of actions.
        /// </summary>
        private IDictionary<string, Func<ActionEventArgs<AppearancePayload>, StreamDeckAction>> ActionFactory { get; } = new Dictionary<string, Func<ActionEventArgs<AppearancePayload>, StreamDeckAction>>();

        /// <summary>
        /// Gets the client.
        /// </summary>
        private StreamDeckClient Client { get; }

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
        /// Handles the <see cref="StreamDeckActionEventReceiver.WillAppear"/> event of the <see cref="Client"/>.
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
                await this._syncRoot.WaitAsync();

                if (!this.ActionCache.TryGetValue(e.Context, out var cacheEntry))
                {
                    // brand new action
                    cacheEntry = this.Create(valueFactory, e);
                    this.ActionCache.Add(e.Context, this.Create(valueFactory, e));
                }
                else if (!this.IsCacheEntryValid(cacheEntry, e))
                {
                    // action exists but does not match, invalidate cache
                    cacheEntry.Action.Dispose();
                    this.ActionCache[e.Context] = this.Create(valueFactory, e);
                }

                _ = this.ActionCache[e.Context].Action.OnWillAppear(e);
            }
            finally
            {
                this._syncRoot.Release();
            }
        }

        /// <summary>
        /// Initializes a new instance of the action associated with the <paramref name="valueFactory"/>, and creates a cache entry.
        /// </summary>
        /// <param name="valueFactory">The value factory to create the new action.</param>
        /// <param name="e">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        /// <returns>The cache entry; the entry is not cached at this stage.</returns>
        private StreamDeckActionCacheEntry Create(Func<ActionEventArgs<AppearancePayload>, StreamDeckAction> valueFactory, ActionEventArgs<AppearancePayload> e)
        {
            // assign a SharpDeck uuid
            var uuid = Guid.NewGuid().ToString("n");
            e.Payload.Settings[SHARP_DECK_UUID_KEY] = uuid;
            _ = this.Client.SetSettingsAsync(e.Context, e.Payload.Settings);

            // initialize the action
            var action = valueFactory(e);
            action.Initialize(e, this.Client);

            return new StreamDeckActionCacheEntry(uuid, action);
        }

        /// <summary>
        /// Determines whether the specified cache entry is valid based on the event arguments supplied by the Stream Deck.
        /// </summary>
        /// <param name="entry">The cache entry.</param>
        /// <param name="e">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        /// <returns><c>true</c> when the cache entry is valid; otherwise <c>false</c>.</returns>
        private bool IsCacheEntryValid(StreamDeckActionCacheEntry entry, ActionEventArgs<AppearancePayload> e)
        {
            return e.Payload.Settings.TryGetString(SHARP_DECK_UUID_KEY, out var uuid)
                && entry.Action.ActionUUID == e.Action
                && entry.Action.Device == e.Device
                && entry.UUID == uuid;
        }

        /// <summary>
        /// Attempts to propagate the event to an associated action, if it exists within the cache.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments parameters.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <param name="getPropagator">The selector to get the propagation event.</param>
        private void TryPropagate<T>(T args, Func<StreamDeckActionEventReceiver, Func<T, Task>> getPropagator)
            where T : IActionEventArgs
        {
            try
            {
                this._syncRoot.Wait();
                if (this.ActionCache.TryGetValue(args.Context, out var cacheEntry))
                {
                    // invoke the propagation
                    getPropagator(cacheEntry.Action)(args).ConfigureAwait(false);
                }
            }
            finally
            {
                this._syncRoot.Release();
            }
        }
    }
}
