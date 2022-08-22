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

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
            => obj is Coordinates other && this.Column == other.Column && this.Row == other.Row;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
            => HashCode.Combine(this.Column, this.Row);

        /// <summary>
        /// Converts this instance to a <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
            => $"{this.Column},{this.Row}";
    }
}
