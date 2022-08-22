namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about an event received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckEventArgs"/> class.
        /// </summary>
        /// <param name="event">The name of the event.</param>
        public StreamDeckEventArgs(string @event)
            : base() => this.Event = @event;

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string Event { get; }
    }
}
