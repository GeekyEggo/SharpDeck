namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides payload information about a title.
    /// </summary>
    public class TitlePayload : ActionPayload
    {
        /// <summary>
        /// Gets the new title.
        /// </summary>
        [JsonInclude]
        public string? Title { get; internal set; }

        /// <summary>
        /// Gets the title parameters describing the title.
        /// </summary>
        [JsonInclude]
        public TitleParameters? TitleParameters { get; internal set; }
    }
}
