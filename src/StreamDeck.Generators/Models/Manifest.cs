namespace StreamDeck.Generators.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a serializable representation of a manifest.json file.
    /// </summary>
    internal class Manifest : ManifestAttribute
    {
        /// <summary>
        /// Gets the actions associated with the plugin.
        /// </summary>
        internal List<ActionAttribute> Actions { get; } = new List<ActionAttribute>();

        /// <summary>
        /// Gets or sets the list of application identifiers to monitor (applications launched or terminated).
        /// </summary>
        internal ApplicationsToMonitor? ApplicationsToMonitor
        {
            get
            {
                if (this.ApplicationsToMonitorMac.Length == 0
                    && this.ApplicationsToMonitorWin.Length == 0)
                {
                    return null;
                }

                return new ApplicationsToMonitor
                {
                    Mac = this.ApplicationsToMonitorMac,
                    Windows = this.ApplicationsToMonitorWin
                };
            }
        }

        /// <summary>
        /// Gets the minimum versions supported by the plugin
        /// </summary>
        internal IEnumerable<SupportedOperatingSystem> OS
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.OSMacMinimumVersion))
                {
                    yield return SupportedOperatingSystem.Mac(this.OSMacMinimumVersion);
                }

                if (string.IsNullOrWhiteSpace(this.OSMacMinimumVersion)
                    || !string.IsNullOrWhiteSpace(this.OSWindowsMinimumVersion))
                {
                    yield return SupportedOperatingSystem.Windows(this.OSWindowsMinimumVersion);
                }
            }
        }

        /// <summary>
        /// Gets or sets an array of profiles. A plugin can have one or more profiles proposed to the user on installation. This lets you create fullscreen plugins.
        /// </summary>
        internal List<ProfileAttribute> Profiles { get; set; } = new List<ProfileAttribute>();

        /// <summary>
        /// Gets or sets the value that indicates which version of the Stream Deck application is required to install the plugin.
        /// </summary>
        internal Software Software => new Software(this.SoftwareMinimumVersion);
    }
}
