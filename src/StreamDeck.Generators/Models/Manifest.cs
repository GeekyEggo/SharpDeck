namespace StreamDeck.Generators.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides a serializable representation of a manifest.json file.
    /// </summary>
    internal class Manifest : ManifestAttribute
    {
        /// <summary>
        /// Private member field for <see cref="Actions"/>
        /// </summary>
        [IgnoreDataMember]
        private readonly Dictionary<string, ActionAttribute> _action = new Dictionary<string, ActionAttribute>();

        /// <summary>
        /// Gets the actions associated with the plugin.
        /// </summary>
        public IReadOnlyCollection<ActionAttribute> Actions => this._action.Values;

        /// <summary>
        /// Gets or sets the list of application identifiers to monitor (applications launched or terminated).
        /// </summary>
        public ApplicationsToMonitor? ApplicationsToMonitor
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
        public IEnumerable<SupportedOperatingSystem> OS
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
                    yield return SupportedOperatingSystem.Windows(string.IsNullOrWhiteSpace(this.OSWindowsMinimumVersion) ? DEFAULT_OS_WINDOWS_MINIMUM_VERSION : this.OSWindowsMinimumVersion);
                }
            }
        }

        /// <summary>
        /// Gets or sets an array of profiles. A plugin can have one or more profiles proposed to the user on installation. This lets you create fullscreen plugins.
        /// </summary>
        public List<ProfileAttribute> Profiles { get; set; } = new List<ProfileAttribute>();

        /// <summary>
        /// Gets or sets the value that indicates which version of the Stream Deck application is required to install the plugin.
        /// </summary>
        public Software Software => new Software(string.IsNullOrWhiteSpace(this.SoftwareMinimumVersion) ? DEFAULT_SOFTWARE_MINIMUM_VERSION : this.SoftwareMinimumVersion);

        /// <summary>
        /// Gets the <see cref="ActionAttribute"/> associated with the <paramref name="uuid"/>.
        /// </summary>
        /// <param name="uuid">The UUID of the action to retrieve.</param>
        /// <param name="action">The action associated with the <paramref name="uuid"/>.</param>
        /// <returns><c>true</c> when an action was found that matched that <paramref name="uuid"/>; otherwise <c>false</c>.</returns>
        public bool TryGetAction(string uuid, out ActionAttribute action)
            => this._action.TryGetValue(uuid, out action);

        /// <summary>
        /// Attempts to add the <paramref name="action" /> to the <see cref="Actions" />; this is only possible when the <see cref="ActionAttribute.UUID"/> is not already assigned to an action.
        /// </summary>
        /// <param name="action">The action to add.</param>
        /// <param name="existing">The value of the existing action if one is already assigned to the given <see cref="ActionAttribute.UUID"/>.</param>
        /// <returns><c>true</c> when the action was successfully added; otherwise <c>false</c>.</returns>
        public bool TryAddAction(ActionAttribute action, out ActionAttribute existing)
        {
            if (this._action.TryGetValue(action.UUID, out existing))
            {
                return false;
            }

            this._action.Add(action.UUID, action);
            return true;
        }
    }
}
