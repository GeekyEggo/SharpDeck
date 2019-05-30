namespace SharpDeck.Events.Received
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides payload information containing settings.
    /// </summary>
    public class SettingsPayload
    {
        /// <summary>
        /// Gets or sets the JSON containing data that you can set and are stored persistently.
        /// </summary>
        public JObject Settings { get; set; }

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
