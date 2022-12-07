namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information for the default layout $B1. This layout includes a title, icon, progress bar (including textual representation).
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LayoutB1
    {
        /// <summary>
        /// Gets or sets the title shown at the top of the layout.
        /// </summary>
        public Text Title { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public Pixmap Icon { get; set; }

        /// <summary>
        /// Gets or sets the indicator.
        /// </summary>
        public Bar Indicator { get; set; }

        /// <summary>
        /// Gets or sets the value shown above the <see cref="Indicator"/>.
        /// </summary>
        public Text Value { get; set; }
    }
}
