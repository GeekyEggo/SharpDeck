namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information about text; used within a layout.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Text : RenderItem
    {
        /// <summary>
        /// Gets or sets the text alignment in the rectangle; default is <see cref="HorizontalAlignment.Center"/>.
        /// </summary>
        public HorizontalAlignment? Alignment { get; set; }

        /// <summary>
        /// Gets or sets the text color; default is white.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Gets or sets the text value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="Text"/>.
        /// </summary>
        /// <param name="value">The <see cref="Text.Value"/>.</param>
        /// <returns>The <see cref="Text"/>.</returns>
        public static implicit operator Text(string value)
            => new Text { Value = value };
    }
}
