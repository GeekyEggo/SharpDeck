namespace StreamDeck.Events
{
    using System.Text.Json.Nodes;

    /// <summary>
    /// Provides payload information relating to a key.
    /// </summary>
    public class KeyPayload : ActionPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPayload"/> class.
        /// </summary>
        /// <param name="coordinates">The coordinates of a triggered action.</param>
        /// <param name="isInMultiAction">A value indicating whether the action is inside a Multi Action.</param>
        /// <param name="settings">The JSON containing data that you can set and are stored persistently.</param>
        public KeyPayload(Coordinates coordinates, bool isInMultiAction, JsonObject settings)
            : base(coordinates, isInMultiAction, settings)
        {
        }

        /// <summary>
        /// Gets a value that is set when the action is triggered with a specific value from a Multi Action.
        /// For example if the user sets the Game Capture Record action to be disabled in a Multi Action, you would see the value 1. Only the value 0 and 1 are valid.
        /// </summary>
        [JsonInclude]
        public int? UserDesiredState { get; internal set; }
    }
}
