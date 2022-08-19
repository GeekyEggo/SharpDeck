namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;
    using StreamDeck.Serialization.Converters;

    /// <summary>
    /// Provides information about a title.
    /// </summary>
    public class TitleParameters
    {
        /// <summary>
        /// Gets the font family for the title.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(FontFamilyJsonConverter))]
        public FontFamily? FontFamily { get; internal set; }

        /// <summary>
        /// Gets the font size for the title.
        /// </summary>
        [JsonInclude]
        public uint? FontSize { get; internal set; }

        /// <summary>
        /// Gets the font style for the title.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(FontStyleJsonConverter))]
        public FontStyle? FontStyle { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the title is underlined.
        /// </summary>
        [JsonInclude]
        public bool? FontUnderline { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the title is visible.
        /// </summary>
        [JsonInclude]
        public bool? ShowTitle { get; internal set; }

        /// <summary>
        /// Gets the vertical alignment of the title. Possible values are "top", "bottom" and "middle".
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(TitleAlignmentJsonConverter))]
        public TitleAlignment? TitleAlignment { get; internal set; }

        /// <summary>
        /// Gets the title color, as a hexidecimal, e.g. #ffffff.
        /// </summary>
        [JsonInclude]
        public string? TitleColor { get; internal set; }
    }
}
