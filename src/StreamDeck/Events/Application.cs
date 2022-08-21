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
        /// Gets the font used by the Stream Deck.
        /// </summary>
        [JsonInclude]
        public string? Font { get; internal set; }

        /// <summary>
        /// Gets the language in which the Stream Deck application is running. Possible values are en, fr, de, es, ja, zh_CN.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(CultureInfoJsonConverter))]
        public CultureInfo? Language { get; internal set; }

        /// <summary>
        /// Gets which platform the Stream Deck application is running
        /// </summary>
        [JsonInclude]
        public string? Platform { get; internal set; }

        /// <summary>
        /// Gets the platform version.
        /// </summary>
        [JsonInclude]
        public Version? PlatformVersion { get; internal set; }

        /// <summary>
        /// Gets the Stream Deck application version.
        /// </summary>
        [JsonInclude]
        public Version? Version { get; internal set; }
    }
}
