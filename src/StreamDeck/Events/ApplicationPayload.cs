namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides payload information relating to an application.
    /// </summary>
    public class ApplicationPayload
    {
        /// <summary>
        /// Gets the identifier of the application that has been launched.
        /// </summary>
        [JsonInclude]
        public string? Application { get; internal set; }
    }
}
