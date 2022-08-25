namespace StreamDeck.Net
{
    using System;

    /// <summary>
    /// Provides information about a message received by a web socket connection.
    /// </summary>
    internal class WebSocketMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public WebSocketMessageEventArgs(in string message)
            => this.Message = message;

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; }
    }
}
