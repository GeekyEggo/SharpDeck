namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information for the default layout $A1.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LayoutA1 : LayoutX1
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public Text Value { get; set; }
    }
}
