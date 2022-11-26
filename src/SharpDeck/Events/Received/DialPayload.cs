namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Provides information about a dial event.
    /// </summary>
    public class DialPayload : SettingsPayload
    {
        /// <summary>
        /// Gets or sets the controller that triggered that action.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the coordinates of a triggered action.
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dial is pressed.
        /// </summary>
        public bool Pressed { get; set; }
    }
}
