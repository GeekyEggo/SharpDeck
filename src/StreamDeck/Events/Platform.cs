namespace StreamDeck.Events
{
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides an enumeration of platforms.
    /// </summary>
    public class Platform : EnumString<Platform>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Platform"/> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        internal Platform(string? platform)
            : base(platform ?? string.Empty)
        {
        }

        /// <summary>
        /// The Mac platform (kESDSDKApplicationInfoPlatformMac).
        /// </summary>
        public const string Mac = "mac";

        /// <summary>
        /// The Windows platform (kESDSDKApplicationInfoPlatformWindows).
        /// </summary>
        public const string Windows = "windows";
    }
}
