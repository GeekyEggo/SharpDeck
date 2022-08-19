namespace StreamDeck.Events
{
    /// <summary>
    /// Provides payload information relating to a key.
    /// </summary>
    public class KeyPayload : AppearancePayload
    {
        /// <summary>
        /// Gets a value that is set when the action is triggered with a specific value from a Multi Action.
        /// For example if the user sets the Game Capture Record action to be disabled in a Multi Action, you would see the value 1. Only the value 0 and 1 are valid.
        /// </summary>
        public int? UserDesiredState { get; internal set; }
    }
}
