namespace SharpDeck.Events.Sent
{
    /// <summary>
    /// Provides payload information relating to logging.
    /// </summary>
    public class LogPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogPayload"/> class.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public LogPayload(string msg)
        {
            this.Message = msg;
        }

        /// <summary>
        /// Gets or sets the message the message to log.
        /// </summary>
        public string Message { get; set; }
    }
}
