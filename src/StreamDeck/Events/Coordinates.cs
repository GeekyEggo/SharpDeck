namespace StreamDeck.Events
{
    /// <summary>
    /// Provides coordinates as represented as <see cref="Column"/> and <see cref="Row"/>.
    /// </summary>
    public struct Coordinates
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinates"/> struct.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="row">The row.</param>
        [JsonConstructor]
        public Coordinates(int column, int row)
        {
            this.Column = column;
            this.Row = row;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets the row.
        public int Row { get; }
    }
}
