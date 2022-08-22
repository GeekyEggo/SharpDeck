namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about an action-based event received from an Elgato Stream Deck.
    /// </summary>
    public class PartialActionEventArgs<TPayload> : StreamDeckEventArgs<TPayload>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialActionEventArgs{TPayload}" /> class.
        /// </summary>
        /// <param name="event">The name of the event.</param>
        /// <param name="payload">The main payload associated with the event.</param>
        /// <param name="action">The actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.</param>
        /// <param name="context">An opaque value identifying the instances action. You will need to pass this opaque value to several APIs like the `setTitle` API.</param>
        public PartialActionEventArgs(string @event, TPayload payload, string action, string context)
            : base(@event, payload)
        {
            this.Action = action;
            this.Context = context;
        }

        /// <summary>
        /// Gets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Gets an opaque value identifying the instances action. You will need to pass this opaque value to several APIs like the `setTitle` API.
        public string Context { get; }
    }
}
