namespace StreamDeck.Events
{
    using System.Globalization;
    using StreamDeck.Serialization.Converters;

    /// <summary>
    /// Provides information about an application.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="font">The font used by Stream Deck.</param>
        /// <param name="language">The language in which the Stream Deck application is running. Possible values are en, fr, de, es, ja, zh_CN..</param>
        /// <param name="platform">The platform the Stream Deck application is running.</param>
        /// <param name="platformVersion">The platform version.</param>
        /// <param name="version">The Stream Deck application version.</param>
        public Application(string font, CultureInfo language, string platform, Version platformVersion, Version version)
        {
            this.Font = font;
            this.Language = language;
            this.Platform = platform;
            this.PlatformVersion = platformVersion;
            this.Version = version;
        }

        /// <summary>
        /// Gets the font used by the Stream Deck.
        /// </summary>
        public string Font { get; }

        /// <summary>
        /// Gets the language in which the Stream Deck application is running. Possible values are en, fr, de, es, ja, zh_CN.
        /// </summary>
        [JsonConverter(typeof(CultureInfoJsonConverter))]
        public CultureInfo Language { get; }

        /// <summary>
        /// Gets the platform the Stream Deck application is running
        /// </summary>
        public string Platform { get; }

        /// <summary>
        /// Gets the platform version.
        /// </summary>
        public Version PlatformVersion { get; }

        /// <summary>
        /// Gets the Stream Deck application version.
        /// </summary>
        public Version Version { get; }
    }
}
