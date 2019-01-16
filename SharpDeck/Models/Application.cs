namespace SharpDeck.Models
{
    using Enums;
    public class Application
    {
        /// <summary>
        /// Gets or sets the language in which the Stream Deck application is running. Possible values are en, fr, de, es, ja, zh_CN.
        /// todo: Create an enumeration, or change to culture info.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets which platform the Stream Deck application is running
        /// </summary>
        public PlatformType Platform { get; set; }

        /// <summary>
        /// Gets or sets the Stream Deck application version.
        /// todo: Change to Version.
        /// </summary>
        public string Version { get; set; }
    }
}
