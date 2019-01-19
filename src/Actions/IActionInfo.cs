namespace SharpDeck.Actions
{
    /// <summary>
    /// Provides information about an instance of an action.
    /// </summary>
    public interface IActionInfo
    {
        /// <summary>
        /// Gets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        string Action { get; }

        /// <summary>
        /// Gets an opaque value identifying the instances action. You will need to pass this opaque value to several APIs like the `setTitle` API.
        /// </summary>
        string Context { get; }

        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        string Device { get; }
    }
}
