namespace StreamDeck.Generators.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides information about applications to monitor.
    /// </summary>
    internal class ApplicationsToMonitor
    {
        /// <summary>
        /// Gets or sets list of application identifiers to monitor on macOS.
        /// </summary>
        [DataMember(Name = SupportedOperatingSystem.MAC)]
        internal string[] Mac { get; set; } = new string[0];

        /// <summary>
        /// Gets or sets list of application identifiers to monitor on Windows.
        /// </summary>
        [DataMember(Name = SupportedOperatingSystem.WINDOWS)]
        internal string[] Windows { get; set; } = new string[0];
    }
}
