namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides payload information relating to an action, and its appearance.
    /// </summary>
    public class AppearancePayload : ActionPayload
    {
        /// <summary>
        /// Gets the state; this is a parameter that is only set when the action has multiple states defined in its manifest.json. The 0-based value contains the current state of the action.
        /// </summary>
        [JsonInclude]
        public int? State { get; internal set; }
    }
}
