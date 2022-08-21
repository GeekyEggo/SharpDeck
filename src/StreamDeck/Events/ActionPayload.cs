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
        [JsonInclude]
        public Coordinates? Coordinates { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the action is inside a Multi Action.
        /// </summary>
        [JsonInclude]
        public bool? IsInMultiAction { get; internal set; }

        /// <summary>
        /// Gets the state; this is a parameter that is only set when the action has multiple states defined in its manifest.json. The 0-based value contains the current state of the action.
        /// </summary>
        [JsonInclude]
        public int? State { get; internal set; }
    }
}
