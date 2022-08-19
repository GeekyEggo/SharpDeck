namespace StreamDeck.Events
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides information about an application.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Gets the language in which the Stream Deck application is running. Possible values are en, fr, de, es, ja, zh_CN.
        /// </summary>
        public CultureInfo? Language { get; internal set; }

        /// <summary>
        /// Gets which platform the Stream Deck application is running
        /// </summary>
        public PlatformType? Platform { get; internal set; }

        /// <summary>
        /// Gets the Stream Deck application version.
        /// </summary>
        public Version? Version { get; internal set; }
    }
}
