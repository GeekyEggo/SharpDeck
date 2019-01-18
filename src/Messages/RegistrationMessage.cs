namespace SharpDeck.Messages
{
    /// <summary>
    /// Provides information about a registration-based message being sent to an Elgato Stream Deck.
    /// </summary>
    public class RegistrationMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationMessage"/> class.
        /// </summary>
        /// <param name="event">The event type that should be used to register the plugin once the WebSocket is opened.</param>
        /// <param name="uuid">The unique identifier string that should be used to register the plugin once the WebSocket is opened.</param>
        public RegistrationMessage(string @event, string uuid)
            : base(@event)
        {
            this.UUID = uuid;
        }

        /// <summary>
        /// Gets or sets the unique identifier string that should be used to register the plugin once the WebSocket is opened.
        /// </summary>
        public string UUID { get; set; }
    }
}
