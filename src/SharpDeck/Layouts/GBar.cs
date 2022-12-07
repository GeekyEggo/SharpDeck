namespace SharpDeck.Layouts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information about a bar with a triangle indicator; used within a layout.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class GBar : Bar
    {
        /// <summary>
        /// Gets or sets the value for the indicator's groove height. The indicator height will be adjusted to fit in the items height; default is 10.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="int"/> to <see cref="GBar"/>.
        /// </summary>
        /// <param name="value">The <see cref="Bar.Value"/>.</param>
        /// <returns>The <see cref="GBar"/>.</returns>
        public static implicit operator GBar(int value)
            => new GBar { Value = value };
    }
}
