namespace StreamDeck.Events
{
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization.Metadata;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides payload information containing settings.
    /// </summary>
    public class SettingsPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPayload"/> class.
        /// </summary>
        /// <param name="settings">The JSON containing data that you can set and are stored persistently.</param>
        public SettingsPayload(JsonObject settings)
            => this.Settings = settings ?? new JsonObject();

        /// <summary>
        /// Gets the JSON containing data that you can set and are stored persistently.
        /// </summary>
        public JsonObject Settings { get; }

        /// <summary>
        /// Gets the settings as the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The desired type of the settings.</typeparam>
        /// <returns>The settings as <typeparamref name="T"/>.</returns>
        public T? GetSettings<T>()
            where T : class
            => this.Settings.Deserialize<T>(StreamDeckJsonContext.Default.Options);

        /// <summary>
        /// Gets the settings as the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The desired type of the settings.</typeparam>
        /// <param name="jsonTypeInfo">The JSON type information.</param>
        /// <returns>The settings as <typeparamref name="T"/>.</returns>
        public T? GetSettings<T>(JsonTypeInfo<T> jsonTypeInfo)
            where T : class
            => this.Settings.Deserialize(jsonTypeInfo);
    }
}
