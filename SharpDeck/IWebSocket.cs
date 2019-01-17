namespace SharpDeck
{
    using Events;
    using System;

    /// <summary>
    /// Provides a light-weight interface for a web socket.
    /// </summary>
    public interface IWebSocket : IDisposable
    {
        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        event EventHandler<WebSocketMessageEventArgs> OnMessage;

        /// <summary>
        /// Connects the web socket.
        /// </summary>
        void Connect();

        /// <summary>
        /// Closes the web socket.
        /// </summary>
        void Close();

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Send(string message);
    }
}
