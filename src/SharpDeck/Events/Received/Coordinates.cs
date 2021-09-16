namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Provides coordinates as represented as <see cref="Coordinates.Column"/> and <see cref="Coordinates.Row"/>.
    /// </summary>
    public class Coordinates
    {
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
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
