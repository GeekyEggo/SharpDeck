namespace SharpDeck.Events.Sent
{
    /// <summary>
    /// Provides information about a message being sent to an Elgato Stream Deck, that contains a payload.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    public class Message<TPayload> : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message{TPayload}"/> class.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="payload">The payload.</param>
        public Message(string @event, TPayload payload)
            : base(@event)
        {
            this.Payload = payload; 
        }

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public TPayload Payload { get; set; }
    }
}
