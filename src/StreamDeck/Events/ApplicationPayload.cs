namespace StreamDeck.Events
{
    /// <summary>
    /// Provides payload information relating to an application.
    /// </summary>
    public class ApplicationPayload
    {
        /// <summary>
        /// Gets the identifier of the application that has been launched.
        /// </summary>
        public string? Application { get; internal set; }
    }
}
