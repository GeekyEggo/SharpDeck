namespace SharpDeck
{
    using Events;
    using System;
    using System.Threading.Tasks;

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
        /// <returns>The task.</returns>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects the web socket.
        /// </summary>
        /// <returns>The task.</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The task.</returns>
        Task SendAsync(string message);
    }
}
