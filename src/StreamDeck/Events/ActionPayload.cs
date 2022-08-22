namespace StreamDeck.Events
{
    using System.Text.Json.Nodes;

    /// <summary>
    /// Provides information about an action.
    /// </summary>
    public class ActionPayload : SettingsPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPayload"/> class.
        /// </summary>
        /// <param name="coordinates">The coordinates of a triggered action.</param>
        /// <param name="isInMultiAction">A value indicating whether the action is inside a Multi Action.</param>
        /// <param name="settings">The JSON containing data that you can set and are stored persistently.</param>
        public ActionPayload(Coordinates coordinates, bool isInMultiAction, JsonObject settings)
            : base(settings)
        {
            this.Coordinates = coordinates;
            this.IsInMultiAction = isInMultiAction;
        }

        /// <summary>
        /// Gets the coordinates of a triggered action.
        /// </summary>
        public Coordinates Coordinates { get; }

        /// <summary>
        /// Gets a value indicating whether the action is inside a Multi Action.
        /// </summary>
        public bool IsInMultiAction { get; }

        /// <summary>
        /// Gets the state; this is a parameter that is only set when the action has multiple states defined in its manifest.json. The 0-based value contains the current state of the action.
        /// </summary>
        [JsonInclude]
        public int? State { get; internal set; }
    }
}
