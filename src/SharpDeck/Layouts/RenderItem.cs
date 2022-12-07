namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides top-level information about a rendered item within a layout.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public abstract class RenderItem
    {
        /// <summary>
        /// Gets or sets value used to define the item background fill color; default is transparent.
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the volume bar is enabled; default is <c>true</c>.
        /// </summary>
        [JsonProperty("enabled")]
        public bool? IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the opacity, where 1 is fully visible, and 0 is invisible; default is 1.
        /// </summary>
        public double? Opacity { get; set; } = 1;

        /// <summary>
        /// Gets or sets the array holding the rectangle coordinates (x, y, w, h) of the defined item. Items with the same zOrder must not overlap. The rectangle must be inside of slot coordinates - (0, 0) x (200, 100).
        /// </summary>
        public int[] Rectangle { get; set; }

        /// <summary>
        /// Gets or sets the non-negative integer in a range [0, 700] defining the z-order of the item. Items with the same zOrder must not overlap; default is 0.
        /// </summary>
        [JsonProperty("zOrder")]
        public uint? ZOrder { get; set; }
    }
}
