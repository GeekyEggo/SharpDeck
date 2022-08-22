namespace SharpDeck.Connectivity
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Events.Received;
    using SharpDeck.Extensions;

    /// <summary>
    /// Provides cache and validity management of <see cref="StreamDeckAction"/>.
    /// </summary>
    public sealed class StreamDeckActionCacheCollection : IStreamDeckActionCacheCollection
    {
        /// <summary>
        /// The key used to assigned a SharpDeck UUID to <see cref="SettingsPayload.Settings"/>.
        /// </summary>
        public const string SHARP_DECK_UUID_KEY = "__sharpDeckUUID";

        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionCacheCollection"/> class.
        /// </summary>
        /// <param name="connection">The connection with the Stream Deck responsible for sending and receiving events and messages.</param>
        public StreamDeckActionCacheCollection(IStreamDeckConnection connection)
            => this.Connection = connection;

        /// <summary>
        /// Gets the connection with the Stream Deck responsible for sending and receiving events and messages.
        /// </summary>
        public IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the cached items.
        /// </summary>
        private IDictionary<string, StreamDeckAction> Items { get; } = new Dictionary<string, StreamDeckAction>();

        /// <summary>
        /// Adds the specified <paramref name="action" /> against the <paramref name="key" />.
        /// </summary>
        /// <param name="key">The <see cref="ActionEventArgs{AppearancePayload}" /> instance containing the event data.</param>
        /// <param name="action">The action to cache.</param>
        public async Task AddAsync(ActionEventArgs<AppearancePayload> key, StreamDeckAction action)
        {
            try
            {
                await _syncRoot.WaitAsync();

                // construct the entry, apply the uuid to the action settings, and add the new item to the cache
                action.SharpDeckUUID = Guid.NewGuid().ToString("n");

                key.Payload.Settings[SHARP_DECK_UUID_KEY] = action.SharpDeckUUID;
                await this.Connection.SetSettingsAsync(key.Context, key.Payload.Settings);

                this.Items.Add(key.Context, action);
            }
            finally
            {
                _syncRoot.Release();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _syncRoot.Wait();

                foreach (var item in this.Items)
                {
                    item.Value.Dispose();
                }

                this.Items.Clear();
            }
            finally
            {
                _syncRoot.Release();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Attempts to get the cached <see cref="StreamDeckAction" /> for the specified <paramref name="key" />
        /// </summary>
        /// <param name="key">The <see cref="IActionEventArgs" /> instance containing the event data.</param>
        /// <param name="action">The result action.</param>
        /// <param name="payload">The optional payload containing the settings; when these are supplied, the cache item is validated.</param>
        /// <returns><c>true</c> when the action was found, and valid if <paramref name="payload"/> were supplied; otherwise <c>false</c>.</returns>
        public bool TryGet(IActionEventArgs key, out StreamDeckAction action, SettingsPayload payload = null)
        {
            try
            {
                _syncRoot.Wait();

                // check if the item exists
                if (!this.Items.TryGetValue(key.Context, out action))
                {
                    return false;
                }

                // ensure the cached item is still valid
                if (payload?.Settings != null
                    && !this.IsCacheEntryValid(action, key, payload.Settings))
                {
                    action.Dispose();
                    this.Items.Remove(key.Context);

                    return false;
                }

                return action != null;
            }
            finally
            {
                _syncRoot.Release();
            }
        }

        /// <summary>
        /// Determines whether the specified cache entry is valid based on the event arguments supplied by the Stream Deck.
        /// </summary>
        /// <param name="action">The cache entry.</param>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        /// <param name="settings">The settings associated with the action</param>
        /// <returns><c>true</c> when the cache entry is valid; otherwise <c>false</c>.</returns>
        private bool IsCacheEntryValid(StreamDeckAction action, IActionEventArgs args, JObject settings)
        {
            return settings.TryGetString(SHARP_DECK_UUID_KEY, out var uuid)
                && action.ActionUUID == args.Action
                && action.Device == args.Device
                && action.SharpDeckUUID == uuid;
        }
    }
}
