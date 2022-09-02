namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information used to identify the instance of an action.
    /// </summary>
    public interface IActionContext
    {
        /// <summary>
        /// Gets the action's unique identifier.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Gets the value identifying the instance's action.
        /// </summary>
        public string Context { get; }
    }
}
