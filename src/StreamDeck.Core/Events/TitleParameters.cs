namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about a title.
    /// </summary>
    public class TitleParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleParameters"/> class.
        /// </summary>
        /// <param name="fontFamily">The font family for the title.</param>
        /// <param name="fontSize">The font size for the title.</param>
        /// <param name="fontStyle">The font style for the title.</param>
        /// <param name="fontUnderline">A value indicating whether the title is underlined.</param>
        /// <param name="showTitle">A value indicating whether the title is visible.</param>
        /// <param name="titleAlignment">The vertical alignment of the title. Possible values are "top", "bottom" and "middle".</param>
        /// <param name="titleColor">The title color, as a hexidecimal, e.g. #ffffff.</param>
        public TitleParameters(
            string fontFamily,
            uint fontSize,
            string fontStyle,
            bool fontUnderline,
            bool showTitle,
            string titleAlignment,
            string titleColor)
        {
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
            this.FontStyle = fontStyle;
            this.FontUnderline = fontUnderline;
            this.ShowTitle = showTitle;
            this.TitleAlignment = titleAlignment;
            this.TitleColor = titleColor;
        }

        /// <summary>
        /// Gets the font family for the title.
        /// </summary>
        public string FontFamily { get; }

        /// <summary>
        /// Gets the font size for the title.
        /// </summary>
        public uint FontSize { get; }

        /// <summary>
        /// Gets the font style for the title.
        /// </summary>
        public string FontStyle { get; }

        /// <summary>
        /// Gets a value indicating whether the title is underlined.
        /// </summary>
        public bool FontUnderline { get; }

        /// <summary>
        /// Gets a value indicating whether the title is visible.
        /// </summary>
        public bool ShowTitle { get; }

        /// <summary>
        /// Gets the vertical alignment of the title. Possible values are "top", "bottom" and "middle".
        /// </summary>
        public string TitleAlignment { get; }

        /// <summary>
        /// Gets the title color, as a hexidecimal, e.g. #ffffff.
        /// </summary>
        public string TitleColor { get; }
    }
}
