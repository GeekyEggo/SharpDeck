namespace SharpDeck.Net
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a light-weight wrapper for <see cref="ClientWebSocket"/>.
    /// </summary>
    public class ClientWebSocketWrapper : IDisposable, IWebSocket
    {
        /// <summary>
        /// The buffer size.
        /// </summary>
        private const int BUFFER_SIZE = 1024 * 1024;

        /// <summary>
        /// The process synchronize root.
        /// </summary>
        private readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientWebSocketWrapper"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public ClientWebSocketWrapper(string uri)
        {
            this.Uri = new Uri(uri);
        }

        /// <summary>
        /// Occurs when the web socket connects.
        /// </summary>
        public event EventHandler Connect;

        /// <summary>
        /// Occurs when the web socket disconnects.
        /// </summary>
        public event EventHandler Disconnect;

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<WebSocketMessageEventArgs> MessageReceived;


        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets the URI.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        private ClientWebSocket WebSocket { get; set; }

        /// <summary>
        /// Connects the web socket.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task ConnectAsync()
        {
            if (this.WebSocket == null)
            {
                this.WebSocket = new ClientWebSocket();

                await this.WebSocket.ConnectAsync(this.Uri, CancellationToken.None);
                _ = this.ReceiveAsync();

                this.Connect?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Disconnects the web socket.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task DisconnectAsync()
        {
            if (this.WebSocket != null)
            {
                var socket = this.WebSocket;
                this.WebSocket = null;

                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
                }

                socket.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public async void Dispose()
        {
            await this.DisconnectAsync();
            this.WebSocket = null;
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The task.</returns>
        public async Task SendAsync(string message)
        {
            if (this.WebSocket == null)
            {
                return;
            }

            try
            {
                await this._syncRoot.WaitAsync();

                var buffer = this.Encoding.GetBytes(message);
                await this.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            finally
            {
                this._syncRoot.Release();
            }
        }

        /// <summary>
        /// Starts listening to message requests from the web socket.
        /// </summary>
        private async Task<WebSocketCloseStatus> ReceiveAsync()
        {
            var buffer = new byte[BUFFER_SIZE];
            var textBuffer = new StringBuilder(BUFFER_SIZE);

            while (this.WebSocket?.State == WebSocketState.Open)
            {
                // await a message
                var result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result == null)
                {
                    continue;
                }

                if (result.MessageType == WebSocketMessageType.Close || (result.CloseStatus != null && result.CloseStatus.HasValue && result.CloseStatus.Value != WebSocketCloseStatus.Empty))
                {
                    // stop listening, and return the close status
                    this.Disconnect?.Invoke(this, EventArgs.Empty);
                    return result.CloseStatus.GetValueOrDefault();
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    // append to the text buffer, and determine if the message has finished
                    textBuffer.Append(this.Encoding.GetString(buffer, 0, result.Count));
                    if (result.EndOfMessage)
                    {
                        this.MessageReceived?.Invoke(this, new WebSocketMessageEventArgs(textBuffer.ToString()));
                        textBuffer.Clear();
                    }
                }
            }

            this.Disconnect?.Invoke(this, EventArgs.Empty);
            return WebSocketCloseStatus.NormalClosure;
        }
    }
}
