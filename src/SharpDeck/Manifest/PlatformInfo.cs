namespace SharpDeck.Manifest
{
    using SharpDeck.Enums;

    /// <summary>
    /// Provides information about supported platforms.
    /// </summary>
    public class PlatformInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformInfo"/> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <param name="minimumVersion">The minimum version.</param>
        public PlatformInfo(PlatformType platform, string minimumVersion)
        {
            this.Platform = platform;
            this.MinimumVersion = minimumVersion;
        }

        /// <summary>
        /// Gets the name of the platform, mac or windows.
        /// </summary>
        public PlatformType Platform { get; }

        /// <summary>
        /// Gets the minimum version of the operating system that the plugin requires. For Windows 10, you can use 10. For macOS 10.11, you can use 10.11.
        /// </summary>
        public string MinimumVersion { get; }
    }
}
