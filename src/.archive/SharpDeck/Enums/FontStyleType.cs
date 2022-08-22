namespace SharpDeck.Enums
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Defines the possible font styles supported by the Elgato Stream Deck.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FontStyleType
    {
        /// <summary>
        /// Regular font.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Bold font.
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Italic font.
        /// </summary>
        Italic = 2,

        /// <summary>
        /// Bold and italic font.
        /// </summary>
        [EnumMember(Value = "Bold Italic")]
        BoldItalic = 3
    }
}
