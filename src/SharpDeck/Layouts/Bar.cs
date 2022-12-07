namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information about a bar; used within a layout.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Bar : RenderItem
    {
        /// <summary>
        /// Gets or sets the width of the border; default is 2.
        /// </summary>
        [JsonProperty("border_w")]
        public int? BorderWidth { get; set; }

        /// <summary>
        /// Gets or sets the bar color or gradient; default is darkGray.
        /// </summary>
        [JsonProperty("bar_bg_c")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the bar border color; default is white.
        /// </summary>
        [JsonProperty("bar_border_c")]
        public string BorderColor { get; set; }

        /// <summary>
        /// Gets or sets fill color used within the bar indicator; default is white..
        /// </summary>
        [JsonProperty("bar_fill_c")]
        public string FillColor { get; set; }

        /// <summary>
        /// Gets or sets the optional value that represents the shape of the bar; recommended is <see cref="BarSubType.Groove"/>.
        /// </summary>
        public BarSubType? SubType { get; set; }

        /// <summary>
        /// Gets or sets the value in the range [0, 100] to display an indicator.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="int"/> to <see cref="Bar"/>.
        /// </summary>
        /// <param name="value">The <see cref="Bar.Value"/>.</param>
        /// <returns>The <see cref="Bar"/>.</returns>
        public static implicit operator Bar(int value)
            => new Bar { Value = value };
    }
}
