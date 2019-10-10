namespace SharpDeck.Connectivity
{
    /// <summary>
    /// Provides information about a cached instance for a <see cref="StreamDeckAction"/>.
    /// </summary>
    public struct StreamDeckActionCacheEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionCacheEntry"/> struct.
        /// </summary>
        /// <param name="uuid">The UUID assigned by SharpDeck.</param>
        /// <param name="action">The action.</param>
        public StreamDeckActionCacheEntry(string uuid, StreamDeckAction action)
        {
            this.UUID = uuid;
            this.Action = action;
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public StreamDeckAction Action { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier assigned by SharpDeck.
        /// </summary>
        public string UUID { get; set; }
    }
}
