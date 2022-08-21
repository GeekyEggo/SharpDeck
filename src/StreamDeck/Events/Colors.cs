namespace StreamDeck.Events
{
    using System.Drawing;
    using StreamDeck.Serialization.Converters;

    /// <summary>
    /// Provides information about the preferred user colors.
    /// </summary>
    public class Colors
    {
        /// <summary>
        /// Gets the button background color, when hovered over.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color? ButtonMouseOverBackgroundColor { get; internal set; }

        /// <summary>
        /// Gets the button background color, when pressed.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color? ButtonPressedBackgroundColor { get; internal set; }

        /// <summary>
        /// Gets the button border color, when pressed.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color? ButtonPressedBorderColor { get; internal set; }

        /// <summary>
        /// Gets the button text color, when pressed.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color? ButtonPressedTextColor { get; internal set; }

        /// <summary>
        /// Gets the highlight color.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color? HighlightColor { get; internal set; }
    }
}
