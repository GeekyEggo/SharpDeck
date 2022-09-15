namespace StreamDeck.Generators.Models
{
    /// <summary>
    /// Provides information about the operating systems supported by the plugin.
    /// </summary>
    internal class SupportedOperatingSystem
    {
        /// <summary>
        /// Identifies a "mac" operating system.
        /// </summary>
        public const string MAC = "mac";

        /// <summary>
        /// Identifies a "windows" operating system.
        /// </summary>
        public const string WINDOWS = "windows";

        /// <summary>
        /// Gets the platform that defines the operating system.
        /// </summary>
        public string Platform { get; private set; } = "";

        /// <summary>
        /// Gets the minimum version required.
        /// </summary>
        public string MinimumVersion { get; private set; } = "";

        /// <summary>
        /// Creates a new <see cref="SupportedOperatingSystem"/> for macOS.
        /// </summary>
        /// <param name="minimumVersion">The minimum version.</param>
        /// <returns>The operating system.</returns>
        public static SupportedOperatingSystem Mac(string minimumVersion)
            => new SupportedOperatingSystem
            {
                Platform = MAC,
                MinimumVersion = minimumVersion
            };

        /// <summary>
        /// Creates a new <see cref="SupportedOperatingSystem"/> for Windows.
        /// </summary>
        /// <param name="minimumVersion">The minimum version.</param>
        /// <returns>The operating system.</returns>
        public static SupportedOperatingSystem Windows(string minimumVersion)
            => new SupportedOperatingSystem
            {
                Platform = WINDOWS,
                MinimumVersion = minimumVersion
            };
    }
}
