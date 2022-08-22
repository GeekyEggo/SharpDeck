namespace SharpDeck.Events.Received
{
    using System;
    using System.Globalization;
    using Enums;

    /// <summary>
    /// Provides information about an application.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Gets or sets the language in which the Stream Deck application is running. Possible values are en, fr, de, es, ja, zh_CN.
        /// </summary>
        public CultureInfo Language { get; set; }

        /// <summary>
        /// Gets or sets which platform the Stream Deck application is running
        /// </summary>
        public PlatformType Platform { get; set; }

        /// <summary>
        /// Gets or sets the Stream Deck application version.
        /// </summary>
        public Version Version { get; set; }
    }
}
