namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about an event received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventArgs<TPayload> : StreamDeckEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckEventArgs{TPayload}"/> class.
        /// </summary>
        /// <param name="event">The name of the event.</param>
        /// <param name="payload">The main payload associated with the event.</param>
        public StreamDeckEventArgs(string @event, TPayload payload)
            : base(@event) => this.Payload = payload;

        /// <summary>
        /// Gets the main payload associated with the event.
        /// </summary>
        public TPayload Payload { get; }
    }
}
