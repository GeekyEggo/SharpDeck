namespace StreamDeck.Events
{
    /// <summary>
    /// /// Provides information about an action-based event received from an Elgato Stream Deck.
    /// </summary>
    public class ActionEventArgs : StreamDeckEventArgs, IActionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventArgs" /> class.
        /// </summary>
        /// <param name="event">The name of the event.</param>
        /// <param name="action">The actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.</param>
        /// <param name="context">The opaque value identifying the instances action. You will need to pass this opaque value to several APIs like the `setTitle` API.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        public ActionEventArgs(string @event, string action, string context, string device)
            : base(@event)
        {
            this.Action = action;
            this.Context = context;
            this.Device = device;
        }

        /// <summary>
        /// Gets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Gets an opaque value identifying the instances action. You will need to pass this opaque value to several APIs like the `setTitle` API.
        /// </summary>
        public string Context { get; }

        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Device { get; }
    }
}
