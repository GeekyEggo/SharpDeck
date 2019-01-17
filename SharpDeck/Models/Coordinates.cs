namespace SharpDeck.Models
{
    /// <summary>
    /// Provides coordinates as represented as <see cref="Coordinates.Column"/> and <see cref="Coordinates.Row"/>; this can also be used to represent a size.
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
    }
}
