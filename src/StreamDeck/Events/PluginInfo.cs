namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about the plugin.
    /// </summary>
    public class PluginInfo
    {
        /// <summary>
        /// Gets the plugin UUID; this matches the one defined within the manifest.json file.
        /// </summary>
        [JsonInclude]
        public string? UUID { get; internal set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        [JsonInclude]
        public Version? Version { get; internal set; }
    }
}
