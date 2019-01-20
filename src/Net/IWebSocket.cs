namespace SharpDeck.Net
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a light-weight interface for a web socket.
    /// </summary>
    public interface IWebSocket : IDisposable
    {
        /// <summary>
        /// Occurs when the web socket connects.
        /// </summary>
        event EventHandler Connect;

        /// <summary>
        /// Occurs when the web socket disconnects.
        /// </summary>
        event EventHandler Disconnect;

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        event EventHandler<WebSocketMessageEventArgs> MessageReceived;

        /// <summary>
        /// Connects the web socket.
        /// </summary>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects the web socket.
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        Task SendAsync(string message);

        /// <summary>
        /// Serializes the value, and sends the message asynchronously.
        /// </summary>
        /// <param name="value">The value to serialize and send.</param>
        Task SendJsonAsync(object value);
    }
}
