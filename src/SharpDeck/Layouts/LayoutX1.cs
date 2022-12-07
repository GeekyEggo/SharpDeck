namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information for the default layout $X1.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LayoutX1
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public Text Title { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public Pixmap Icon { get; set; }
    }
}
