namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about an action-based event received from an Elgato Stream Deck.
    /// </summary>
    public class ActionEventArgs<TPayload> : PartialActionEventArgs<TPayload>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventArgs{TPayload}" /> class.
        /// </summary>
        /// <param name="event">The name of the event.</param>
        /// <param name="payload">The main payload associated with the event.</param>
        /// <param name="action">The actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.</param>
        /// <param name="context">An opaque value identifying the instances action. You will need to pass this opaque value to several APIs like the `setTitle` API.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        public ActionEventArgs(string @event, TPayload payload, string action, string context, string device)
            : base(@event, payload, action, context) => this.Device = device;

        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        public string Device { get; }
    }
}
