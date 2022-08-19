namespace StreamDeck.Events
{
    using System.Runtime.Serialization;

    /// <summary>
    /// An enumeration of fonts available to the Elgato Stream Deck.
    /// </summary>
    public enum FontFamilyType
    {
        /// <summary>
        /// Arial font.
        /// </summary>
        [EnumMember(Value = "Arial")]
        Arial = 1,

        /// <summary>
        /// Arial Black font.
        /// </summary>
        [EnumMember(Value = "Arial Black")]
        ArialBlack = 2,

        /// <summary>
        /// Comic Sans MS font.
        /// </summary>
        [EnumMember(Value = "Comic Sans MS")]
        ComicSansMS = 3,

        /// <summary>
        /// Courier font.
        /// </summary>
        [EnumMember(Value = "Courier")]
        Courier = 4,

        /// <summary>
        /// Courier New font.
        /// </summary>
        [EnumMember(Value = "Courier New")]
        CourierNew = 5,

        /// <summary>
        /// Georgia font.
        /// </summary>
        [EnumMember(Value = "Georgia")]
        Georgia = 6,

        /// <summary>
        /// Impact font.
        /// </summary>
        [EnumMember(Value = "Impact")]
        Impact = 7,

        /// <summary>
        /// Microsoft Sans Serif font.
        /// </summary>
        [EnumMember(Value = "Microsoft Sans Serif")]
        MicrosoftSansSerif = 8,

        /// <summary>
        /// Symbol font.
        /// </summary>
        [EnumMember(Value = "Symbol")]
        Symbol = 9,

        /// <summary>
        /// Tahoma font.
        /// </summary>
        [EnumMember(Value = "Tahoma")]
        Tahoma = 10,

        /// <summary>
        /// Times New Roman font.
        /// </summary>
        [EnumMember(Value = "Times New Roman")]
        TimesNewRoman = 11,

        /// <summary>
        /// Trebuchet MS font.
        /// </summary>
        [EnumMember(Value = "Trebuchet MS")]
        TrebuchetMS = 12,

        /// <summary>
        /// Verdana font.
        /// </summary>
        [EnumMember(Value = "Verdana")]
        Verdana = 13,

        /// <summary>
        /// Webdings font.
        /// </summary>
        [EnumMember(Value = "Webdings")]
        Webdings = 14,

        /// <summary>
        /// Wingdings font.
        /// </summary>
        [EnumMember(Value = "Wingdings")]
        Wingdings = 15,
    }
}
