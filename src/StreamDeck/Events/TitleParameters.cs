namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about a title.
    /// </summary>
    public class TitleParameters
    {
        /// <summary>
        /// Gets the font family for the title.
        /// </summary>
        public string? FontFamily { get; internal set; }

        /// <summary>
        /// Gets the font size for the title.
        /// </summary>
        public uint? FontSize { get; internal set; }

        /// <summary>
        /// Gets the font style for the title.
        /// </summary>
        public string? FontStyle { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the title is underlined.
        /// </summary>
        public bool? FontUnderline { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the title is visible.
        /// </summary>
        public bool? ShowTitle { get; internal set; }

        /// <summary>
        /// Gets the vertical alignment of the title. Possible values are "top", "bottom" and "middle".
        /// </summary>
        public TitleAlignment? TitleAlignment { get; internal set; }

        /// <summary>
        /// Gets the title color, as a hexidecimal, e.g. #ffffff.
        /// </summary>
        public string? TitleColor { get; internal set; }
    }
}
