namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides information about the plugin.
    /// </summary>
    public class PluginInfo
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        [JsonInclude]
        public string? Version { get; internal set; }
    }
}
