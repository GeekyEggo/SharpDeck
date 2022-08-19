namespace StreamDeck.Events
{
    /// <summary>
    /// Provides payload information about a title.
    /// </summary>
    public class TitlePayload : ActionPayload
    {
        /// <summary>
        /// Gets the new title.
        /// </summary>
        public string? Title { get; internal set; }

        /// <summary>
        /// Gets the title parameters describing the title.
        /// </summary>
        public TitleParameters? TitleParameters { get; internal set; }
    }
}
