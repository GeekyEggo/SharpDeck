namespace StreamDeck.Events
{
    /// <summary>
    /// Provides a size as represented by <see cref="Columns"/> and <see cref="Rows"/>.
    /// </summary>
    public class Size
    {
        /// <summary>
        /// Gets the columns.
        /// </summary>
        public int? Columns { get; internal set; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        public int? Rows { get; internal set; }
    }
}
