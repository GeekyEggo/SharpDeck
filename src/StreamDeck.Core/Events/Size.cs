namespace StreamDeck.Events
{
    /// <summary>
    /// Provides a size as represented by <see cref="Columns"/> and <see cref="Rows"/>.
    /// </summary>
    public struct Size
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="rows">The rows.</param>
        [JsonConstructor]
        public Size(int columns, int rows)
        {
            this.Columns = columns;
            this.Rows = rows;
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        public int Rows { get; }
    }
}
