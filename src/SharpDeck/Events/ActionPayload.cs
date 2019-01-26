namespace SharpDeck.Events
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides information about an action.
    /// </summary>
    public class ActionPayload
    {
        /// <summary>
        /// Gets or sets the coordinates of a triggered action.
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the action is inside a Multi Action.
        /// </summary>
        public bool IsInMultiAction { get; set; }

        /// <summary>
        /// Gets or sets the JSON containing data that you can set and are stored persistently.
        /// </summary>
        public JObject Settings { get; set; }

        /// <summary>
        /// Gets or sets the state; this is a parameter that is only set when the action has multiple states defined in its manifest.json. The 0-based value contains the current state of the action.
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Gets the settings as the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The desired type of the settings.</typeparam>
        /// <returns>The settings as <typeparamref name="T"/>.</returns>
        public T GetSettings<T>()
            where T : class
            => this.Settings.ToObject<T>();
    }
}
