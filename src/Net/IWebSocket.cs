namespace SharpDeck.Net
{
    using System;
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a light-weight interface for a web socket.
    /// </summary>
    public interface IWebSocket : IDisposable
    {
        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        event EventHandler<WebSocketMessageEventArgs> MessageReceived;

        /// <summary>
        /// Connects the web socket.
        /// </summary>
        /// <returns>The the web socket state; this will return <see cref="WebSocketState.None"/> when the web socket is already connected.</returns>
        Task<WebSocketState> ConnectAsync();

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
