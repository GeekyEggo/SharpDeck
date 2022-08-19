namespace StreamDeck.Events
{
    using System;
    using System.Globalization;
    using System.Text.Json.Serialization;
    using StreamDeck.Serialization.Converters;

    /// <summary>
    /// Provides information about an application.
    /// </summary>
    public class Application
    {
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
        [JsonConverter(typeof(PlatformJsonConverter))]
        public Platform? Platform { get; internal set; }

        /// <summary>
        /// Gets the Stream Deck application version.
        /// </summary>
        [JsonInclude]
        public Version? Version { get; internal set; }
    }
}
