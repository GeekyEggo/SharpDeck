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
    public class StreamDeckActionCache : IStreamDeckActionCache
    {
        /// <summary>
        /// The key used to assigned a SharpDeck UUID to <see cref="SettingsPayload.Settings"/>.
        /// </summary>
        private const string SHARP_DECK_UUID_KEY = "__sharpDeckUUID";

        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionCache"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public StreamDeckActionCache(IStreamDeckClient client)
        {
            this.Client = client;
        }

        /// <summary>
        /// Gets the Stream Deck client.
        /// </summary>
        public IStreamDeckClient Client { get; }

        /// <summary>
        /// Gets the cached items.
        /// </summary>
        private Dictionary<string, StreamDeckActionCacheEntry> Items { get; } = new Dictionary<string, StreamDeckActionCacheEntry>();

        /// <summary>
        /// Adds the specified <paramref name="action" /> against the <paramref name="key" />.
        /// </summary>
        /// <param name="key">The <see cref="ActionEventArgs{AppearancePayload}" /> instance containing the event data.</param>
        /// <param name="action">The action to cache.</param>
        public void Add(ActionEventArgs<AppearancePayload> key, StreamDeckAction action)
        {
            try
            {
                _syncRoot.Wait();

                // construct the entry, apply the uuid to the action settings, and add the new item to the cache
                var entry = new StreamDeckActionCacheEntry(Guid.NewGuid().ToString("n"), action);

                key.Payload.Settings[SHARP_DECK_UUID_KEY] = entry.UUID;
                _ = this.Client.SetSettingsAsync(key.Context, key.Payload.Settings);

                this.Items.Add(key.Context, entry);
            }
            finally
            {
                _syncRoot.Release();
            }
        }

        /// <summary>
        /// Attempts to get the cached <see cref="StreamDeckAction" /> for the specified <paramref name="key" />
        /// </summary>
        /// <param name="key">The <see cref="IActionEventArgs" /> instance containing the event data.</param>
        /// <param name="action">The result action.</param>
        /// <returns><c>true</c> when the action was found; otherwise <c>false</c>.</returns>
        public bool TryGet(IActionEventArgs key, out StreamDeckAction action)
            => this.TryGet<SettingsPayload>(key, out action, null);

        /// <summary>
        /// Attempts to get the cached <see cref="StreamDeckAction" /> for the specified <paramref name="key" />
        /// </summary>
        /// <param name="key">The <see cref="IActionEventArgs" /> instance containing the event data.</param>
        /// <param name="action">The result action.</param>
        /// <param name="payload">The optional payload containing the settings; when these are supplied, the cache item is validated.</param>
        /// <returns><c>true</c> when the action was found, and valid if <paramref name="payload"/> were supplied; otherwise <c>false</c>.</returns>
        public bool TryGet<T>(IActionEventArgs key, out StreamDeckAction action, T payload)
            where T : SettingsPayload
        {
            try
            {
                _syncRoot.Wait();
                action = null;

                // check if the item exists
                if (!this.Items.TryGetValue(key.Context, out var cacheEntry))
                {
                    return false;
                }

                // ensure the cached item is still valid
                if (payload?.Settings != null
                    && !this.IsCacheEntryValid(cacheEntry, key, payload.Settings))
                {
                    cacheEntry.Action.Dispose();
                    this.Items.Remove(key.Context);

                    return false;
                }

                action = cacheEntry?.Action;
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
        /// <param name="entry">The cache entry.</param>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        /// <param name="settings">The settings associated with the action</param>
        /// <returns><c>true</c> when the cache entry is valid; otherwise <c>false</c>.</returns>
        private bool IsCacheEntryValid(StreamDeckActionCacheEntry entry, IActionEventArgs args, JObject settings)
        {
            return settings.TryGetString(SHARP_DECK_UUID_KEY, out var uuid)
                && entry.Action.ActionUUID == args.Action
                && entry.Action.Device == args.Device
                && entry.UUID == uuid;
        }
    }
}
