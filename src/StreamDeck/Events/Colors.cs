namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about the preferred user colors.
    /// </summary>
    public class Colors
    {
        /// <summary>
        /// Gets the button background color, when hovered over.
        /// </summary>
        [JsonInclude]
        public string? ButtonMouseOverBackgroundColor { get; internal set; }

        /// <summary>
        /// Gets the button background color, when pressed.
        /// </summary>
        [JsonInclude]
        public string? ButtonPressedBackgroundColor { get; internal set; }

        /// <summary>
        /// Gets the button border color, when pressed.
        /// </summary>
        [JsonInclude]
        public string? ButtonPressedBorderColor { get; internal set; }

        /// <summary>
        /// Gets the button text color, when pressed.
        /// </summary>
        [JsonInclude]
        public string? ButtonPressedTextColor { get; internal set; }

        /// <summary>
        /// Gets the highlight color.
        /// </summary>
        [JsonInclude]
        public string? HighlightColor { get; internal set; }
    }
}
