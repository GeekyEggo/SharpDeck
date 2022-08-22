namespace StreamDeck.Payloads
{
    /// <summary>
    /// Provides information about an action-based message being sent to an Elgato Stream Deck.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    public class ActionMessage<TPayload> : ContextMessage<TPayload>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMessage{TPayload}"/> class.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="context">An opaque value identifying the instances action.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">The payload.</param>
        public ActionMessage(string @event, string context, string action, TPayload payload)
            : base(@event, context, payload) => this.Action = action;

        /// <summary>
        /// Gets the action unique identifier.
        /// </summary>
        public string Action { get; }
    }
}
