namespace SharpDeck.Events.Received
{
    /// <summary>
    /// /// Provides information about an action-based event received from an Elgato Stream Deck.
    /// </summary>
    public class ActionEventArgs : StreamDeckEventArgs
    {
        /// <summary>
        /// Gets or sets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets an opaque value identifying the instances action. You will need to pass this opaque value to several APIs like the `setTitle` API.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Device { get; set; }
    }
}
