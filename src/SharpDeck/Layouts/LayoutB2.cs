namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information for the default layout $B2.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LayoutB2 : LayoutX1
    {
        /// <summary>
        /// Gets or sets the indicator.
        /// </summary>
        public GBar Indicator { get; set; }

        /// <summary>
        /// Gets or sets the value shown above the <see cref="Indicator"/>.
        /// </summary>
        public Text Value { get; set; }
    }
}
