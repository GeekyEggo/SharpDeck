namespace StreamDeck.Events.Sent
{
    /// <summary>
    /// Provides information about a message being sent to an Elgato Stream Deck.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="event">The event.</param>
        public Message(string @event)
            => this.Event = @event;

        /// <summary>
        /// Gets the event.
        /// </summary>
        public string Event { get; }
    }
}
