namespace SharpDeck.Layouts
{
    /// <summary>
    /// Provides information about a pixel-map; used within a layout.
    /// </summary>
    public class Pixmap : RenderItem
    {
        /// <summary>
        /// Gets or sets the value used to provide an image path, or image itself as a base64 encoded data.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="Pixmap"/>.
        /// </summary>
        /// <param name="value">The <see cref="Pixmap.Value"/>.</param>
        /// <returns>The <see cref="Pixmap"/>.</returns>
        public static implicit operator Pixmap(string value)
            => new Pixmap { Value = value };
    }
}
