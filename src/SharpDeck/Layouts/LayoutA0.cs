namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information for the default layout $A0.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LayoutA0
    {
        /// <summary>
        /// Gets or sets the canvas.
        /// </summary>
        public Pixmap Canvas { get; set; }

        /// <summary>
        /// Gets or sets the full-canvas.
        /// </summary>
        [JsonProperty("full-canvas")]
        public Pixmap FullCanvas { get; set; }

        /// <summary>
        /// Gets or sets the title the layout.
        /// </summary>
        public Text Title { get; set; }
    }
}
