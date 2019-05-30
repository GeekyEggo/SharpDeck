namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Provides payload information about a title.
    /// </summary>
    public class TitlePayload
    {
        /// <summary>
        /// Gets or sets the new title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the title parameters describing the title.
        /// </summary>
        public TitleParameters TitleParameters { get; set; }
    }
}
