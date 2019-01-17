namespace SharpDeck.Events
{
    /// <summary>
    /// Provides information about an event received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventArgs<TPayload> : StreamDeckEventArgs
    {
        /// <summary>
        /// Gets or sets the main payload associated with the event.
        /// </summary>
        public TPayload Payload { get; set; }
    }
}
