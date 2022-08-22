namespace StreamDeck.Events
{
    /// <summary>
    /// Provides payload information relating to an application.
    /// </summary>
    public class ApplicationPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationPayload"/> class.
        /// </summary>
        /// <param name="application">The identifier of the application that has been launched.</param>
        public ApplicationPayload(string application)
            => this.Application = application;

        /// <summary>
        /// Gets the identifier of the application that has been launched.
        /// </summary>
        public string Application { get; }
    }
}
