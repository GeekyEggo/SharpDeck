namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about the plugin.
    /// </summary>
    public class PluginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInfo"/> class.
        /// </summary>
        /// <param name="uuid">The plugin UUID; this matches the one defined within the manifest.json file.</param>
        /// <param name="version">The version.</param>
        public PluginInfo(string uuid, Version version)
        {
            this.UUID = uuid;
            this.Version = version;
        }

        /// <summary>
        /// Gets the plugin UUID; this matches the one defined within the manifest.json file.
        /// </summary>
        public string UUID { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public Version Version { get; }
    }
}
