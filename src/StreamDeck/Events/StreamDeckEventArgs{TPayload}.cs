namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about an event received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventArgs<TPayload> : StreamDeckEventArgs
    {
        /// <summary>
        /// Gets the main payload associated with the event.
        /// </summary>
        [JsonInclude]
        public TPayload? Payload { get; internal set; }
    }
}
