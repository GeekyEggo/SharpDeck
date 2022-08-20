namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about an event received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        [JsonInclude]
        public string? Event { get; internal set; }
    }
}
