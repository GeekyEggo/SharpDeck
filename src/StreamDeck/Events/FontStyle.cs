namespace StreamDeck.Events.Received
{
    using StreamDeck.Serialization;

    /// <summary>
    /// Defines the possible font styles supported by the Elgato Stream Deck.
    /// </summary>
    public class FontStyle : EnumString<FontStyle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontStyle"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        internal FontStyle(string? style)
            : base(style ?? Regular)
        {
        }

        /// <summary>
        /// Regular font.
        /// </summary>
        public const string Regular = nameof(Regular);

        /// <summary>
        /// Bold font.
        /// </summary>
        public const string Bold = nameof(Bold);

        /// <summary>
        /// Italic font.
        /// </summary>
        public const string Italic = nameof(Italic);

        /// <summary>
        /// Bold and italic font.
        /// </summary>
        public const string BoldItalic = "Bold Italic";
    }
}
