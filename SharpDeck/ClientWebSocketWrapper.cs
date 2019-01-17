namespace SharpDeck
{
    using Events;
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
        /// Initializes a new instance of the <see cref="ClientWebSocketWrapper"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public ClientWebSocketWrapper(string uri)
        {
            this.Uri = new Uri(uri);
            this.WebSocket = new ClientWebSocket();
        }

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<WebSocketMessageEventArgs> OnMessage;

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets the URI.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets or sets the cancellation token source.
        /// </summary>
        private CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        private ClientWebSocket WebSocket { get; set; }

        /// <summary>
        /// Connects the web socket.
        /// </summary>
        public void Connect()
        {
            if (this.WebSocket.State == WebSocketState.Open)
            {
                return;
            }

            this.WebSocket.ConnectAsync(this.Uri, this.CancellationTokenSource.Token)
                .Wait(this.CancellationTokenSource.Token);

            this.StartReceiving();
        }

        /// <summary>
        /// Closes the web socket.
        /// </summary>
        public void Close()
        {
            if (this.WebSocket?.State == WebSocketState.Open)
            {
                this.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.CancellationTokenSource.Cancel();
            this.Close();
            this.WebSocket = null;
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(string message)
            => this.WebSocket.SendAsync(new ArraySegment<byte>(this.Encoding.GetBytes(message)), WebSocketMessageType.Binary, false, this.CancellationTokenSource.Token).Wait();

        /// <summary>
        /// Starts listening to message requests from the web socket.
        /// </summary>
        private void StartReceiving()
        {
            Task.Run(async () =>
            {
                while (this.WebSocket?.State == WebSocketState.Open)
                {
                    var buffer = new byte[1024];
                    var result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), this.CancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await this.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, this.CancellationTokenSource.Token);
                    }
                    else
                    {
                        this.OnMessage?.Invoke(this, new WebSocketMessageEventArgs(this.Encoding.GetString(buffer).TrimEnd('\0')));
                    }
                }
            });
        }
    }
}
