namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information about the font used in a <see cref="Text"/> item.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Font
    {
        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        public string Family { get; set; }

        /// <summary>
        /// Gets or sets the font size, in pixels.
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Gets or sets the font weight; an integer value between 100 and 1000 or the string with a name of typographical weight.
        /// </summary>
        public int? Weight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font is bold; when <c>true</c> the weight will be 600 (aka SemiBold).
        /// </summary>
        [JsonProperty("bold")]
        public bool? IsBold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font is italic.
        /// </summary>
        [JsonProperty("italic")]
        public bool? IsItalic { get; set; }
    }
}
