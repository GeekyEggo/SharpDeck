namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides a size as represented by <see cref="Columns"/> and <see cref="Rows"/>.
    /// </summary>
    public class Size
    {
        /// <summary>
        /// Gets the columns.
        /// </summary>
        [JsonInclude]
        public int? Columns { get; internal set; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        [JsonInclude]
        public int? Rows { get; internal set; }
    }
}
