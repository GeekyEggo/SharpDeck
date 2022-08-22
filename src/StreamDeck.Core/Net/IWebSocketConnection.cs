namespace StreamDeck.Net
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a light-weight wrapper for <see cref="ClientWebSocket"/>.
    /// </summary>
    internal interface IWebSocketConnection : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        event EventHandler<WebSocketMessageEventArgs>? MessageReceived;

        /// <summary>
        /// Connects the web socket to the specified <paramref name="uri" />.
        /// </summary>
        /// <param name="uri">The URI to connect to.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        Task ConnectAsync(string uri, CancellationToken cancellationToken = default);

        /// <summary>
        /// Disconnects the web socket.
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Sends the specified message message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        Task SendAsync(string message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Waits for the underlying connection to disconnect asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of the live connection.</returns>
        Task WaitForDisconnectAsync(CancellationToken cancellationToken = default);
    }
}
