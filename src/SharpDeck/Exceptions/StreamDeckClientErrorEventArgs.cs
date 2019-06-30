namespace SharpDeck.Exceptions
{
    using System;

    /// <summary>
    /// Provides information about an error encountered by <see cref="StreamDeckClient"/>.
    /// </summary>
    public class StreamDeckClientErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckClientErrorEventArgs" /> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="webSocketMessage">The web socket message</param>
        /// <param name="context">The optional context of the action.</param>
        public StreamDeckClientErrorEventArgs(Exception exception, string webSocketMessage, string context = "")
        {
            this.Context = context;
            this.Exception = exception;
            this.WebSocketMessage = webSocketMessage;
        }

        /// <summary>
        /// Gets the action context for the source of the error.
        /// </summary>
        public string Context { get; }

        /// <summary>
        /// Gets the errors exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the web socket message.
        /// </summary>
        public string WebSocketMessage { get; }
    }
}
