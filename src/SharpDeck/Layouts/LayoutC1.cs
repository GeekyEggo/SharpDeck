namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information for the default layout $C1.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LayoutC1
    {
        /// <summary>
        /// Gets or sets icon one.
        /// </summary>
        [JsonProperty("icon1")]
        public Pixmap IconOne { get; set; }

        /// <summary>
        /// Gets or sets icon two.
        /// </summary>
        [JsonProperty("icon2")]
        public Pixmap IconTwo { get; set; }

        /// <summary>
        /// Gets or sets indicator one.
        /// </summary>
        [JsonProperty("indicator1")]
        public Bar IndicatorOne { get; set; }

        /// <summary>
        /// Gets or sets indicator two.
        /// </summary>
        [JsonProperty("indicator2")]
        public Bar IndicatorTwo { get; set; }

        /// <summary>
        /// Gets or sets the title the layout.
        /// </summary>
        public Text Title { get; set; }
    }
}
