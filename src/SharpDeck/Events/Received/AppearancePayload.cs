namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Provides payload information relating to an action, and its appearance.
    /// </summary>
    public class AppearancePayload : ActionPayload
    {
        /// <summary>
        /// Gets or sets the state; this is a parameter that is only set when the action has multiple states defined in its manifest.json. The 0-based value contains the current state of the action.
        /// </summary>
        public int State { get; set; }
    }
}
