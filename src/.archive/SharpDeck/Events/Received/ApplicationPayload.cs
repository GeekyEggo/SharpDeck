namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Provides payload information relating to an application.
    /// </summary>
    public class ApplicationPayload
    {
        /// <summary>
        /// Gets or sets the identifier of the application that has been launched.
        /// </summary>
        public string Application { get; set; }
    }
}
