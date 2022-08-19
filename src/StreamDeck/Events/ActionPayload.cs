namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;

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
    }
}
