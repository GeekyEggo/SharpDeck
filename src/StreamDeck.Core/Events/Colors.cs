namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about the preferred user colors.
    /// </summary>
    public class Colors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Colors"/> class.
        /// </summary>
        /// <param name="buttonMouseOverBackgroundColor">The button background color, when hovered over.</param>
        /// <param name="buttonPressedBackgroundColor">The button background color, when pressed.</param>
        /// <param name="buttonPressedBorderColor">The button border color, when pressed.</param>
        /// <param name="buttonPressedTextColor">The button text color, when pressed.</param>
        /// <param name="highlightColor">The highlight color.</param>
        public Colors(string buttonMouseOverBackgroundColor, string buttonPressedBackgroundColor, string buttonPressedBorderColor, string buttonPressedTextColor, string highlightColor)
        {
            this.ButtonMouseOverBackgroundColor = buttonMouseOverBackgroundColor;
            this.ButtonPressedBackgroundColor = buttonPressedBackgroundColor;
            this.ButtonPressedBorderColor = buttonPressedBorderColor;
            this.ButtonPressedTextColor = buttonPressedTextColor;
            this.HighlightColor = highlightColor;
        }

        /// <summary>
        /// Gets the button background color, when hovered over.
        /// </summary>
        public string ButtonMouseOverBackgroundColor { get; }

        /// <summary>
        /// Gets the button background color, when pressed.
        /// </summary>
        public string ButtonPressedBackgroundColor { get; }

        /// <summary>
        /// Gets the button border color, when pressed.
        /// </summary>
        public string ButtonPressedBorderColor { get; }

        /// <summary>
        /// Gets the button text color, when pressed.
        /// </summary>
        public string ButtonPressedTextColor { get; }

        /// <summary>
        /// Gets the highlight color.
        /// </summary>
        public string HighlightColor { get; }
    }
}
