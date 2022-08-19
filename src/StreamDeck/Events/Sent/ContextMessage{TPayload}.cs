namespace StreamDeck.Events.Sent
{
    /// <summary>
    /// Provides information about a context-based message being sent to an Elgato Stream Deck, that contains a payload.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    public class ContextMessage<TPayload> : Message<TPayload>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMessage{TPayload}" /> class.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="context">The context.</param>
        /// <param name="payload">The payload</param>
        public ContextMessage(string @event, string context, TPayload payload)
            : base(@event, payload) => this.Context = context;

        /// <summary>
        /// Gets the context.
        /// </summary>
        public string Context { get; }
    }
}
