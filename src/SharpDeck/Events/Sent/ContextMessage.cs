namespace SharpDeck.Events.Sent
{
    /// <summary>
    /// Provides information about a context-based message being sent to an Elgato Stream Deck.
    /// </summary>
    public class ContextMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMessage"/> class.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="context">The context.</param>
        public ContextMessage(string @event, string context)
            : base(@event)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public string Context { get; set; }
    }
}
