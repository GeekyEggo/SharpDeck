namespace StreamDeck.Events
{
    using StreamDeck.Serialization;

    /// <summary>
    /// An enumeration of fonts available to the Elgato Stream Deck.
    /// </summary>
    public class FontFamily : EnumString<FontFamily>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamily"/> class.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        internal FontFamily(string? name)
            : base(name ?? FontFamily.Default)
        {
        }

        /// <summary>
        /// Default font.
        /// </summary>
        public const string Default = "";

        /// <summary>
        /// Arial font.
        /// </summary>
        public const string Arial = nameof(Arial);

        /// <summary>
        /// Arial Black font.
        /// </summary>
        public const string ArialBlack = "Arial Black";

        /// <summary>
        /// Comic Sans MS font.
        /// </summary>
        public const string ComicSansMS = "Comic Sans MS";

        /// <summary>
        /// Courier font.
        /// </summary>
        public const string Courier = nameof(Courier);

        /// <summary>
        /// Courier New font.
        /// </summary>
        public const string CourierNew = "Courier New";

        /// <summary>
        /// Georgia font.
        /// </summary>
        public const string Georgia = nameof(Georgia);

        /// <summary>
        /// Impact font.
        /// </summary>
        public const string Impact = nameof(Impact);

        /// <summary>
        /// Microsoft Sans Serif font.
        /// </summary>
        public const string MicrosoftSansSerif = "Microsoft Sans Serif";

        /// <summary>
        /// Symbol font.
        /// </summary>
        public const string Symbol = nameof(Symbol);

        /// <summary>
        /// Tahoma font.
        /// </summary>
        public const string Tahoma = nameof(Tahoma);

        /// <summary>
        /// Times New Roman font.
        /// </summary>
        public const string TimesNewRoman = "Times New Roman";

        /// <summary>
        /// Trebuchet MS font.
        /// </summary>
        public const string TrebuchetMS = "Trebuchet MS";

        /// <summary>
        /// Verdana font.
        /// </summary>
        public const string Verdana = nameof(Verdana);

        /// <summary>
        /// Webdings font.
        /// </summary>
        public const string Webdings = nameof(Webdings);

        /// <summary>
        /// Wingdings font.
        /// </summary>
        public const string Wingdings = nameof(Wingdings);

        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="FontFamily"/>.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator FontFamily(string fontFamily)
            => new FontFamily(fontFamily);
    }
}
