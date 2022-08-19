namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about an action.
    /// </summary>
    public class ActionPayload : SettingsPayload
    {
        /// <summary>
        /// Gets the coordinates of a triggered action.
        /// </summary>
        public Coordinates? Coordinates { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the action is inside a Multi Action.
        /// </summary>
        public bool? IsInMultiAction { get; internal set; }
    }
}
