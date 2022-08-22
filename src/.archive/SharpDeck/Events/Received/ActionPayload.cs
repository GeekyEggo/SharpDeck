namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Provides information about an action.
    /// </summary>
    public class ActionPayload : SettingsPayload
    {
        /// <summary>
        /// Gets or sets the coordinates of a triggered action.
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the action is inside a Multi Action.
        /// </summary>
        public bool IsInMultiAction { get; set; }
    }
}
