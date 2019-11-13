namespace SharpDeck.Connectivity.Net
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides a light-weight wrapper for <see cref="ClientWebSocket"/>.
    /// </summary>
    public class WebSocketConnection : IDisposable
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
        /// Initializes a new instance of the <see cref="WebSocketConnection" /> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="jsonSettings">The JSON settings.</param>
        public WebSocketConnection(string uri, JsonSerializerSettings jsonSettings = null)
        {
            this.JsonSettings = jsonSettings;
            this.Uri = new Uri(uri);
        }

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<WebSocketMessageEventArgs> MessageReceived;

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets the JSON settings.
        /// </summary>
        public JsonSerializerSettings JsonSettings { get; }

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
        public async Task ConnectAsync()
        {
            if (this.WebSocket == null)
            {
                this.WebSocket = new ClientWebSocket();
                await this.WebSocket.ConnectAsync(this.Uri, CancellationToken.None);
            }
        }

        /// <summary>
        /// Disconnects the web socket.
        /// </summary>
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
        public void Dispose()
        {
            _ = this.DisconnectAsync();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Receive data as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<WebSocketCloseStatus> ReceiveAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[BUFFER_SIZE];
            var textBuffer = new StringBuilder(BUFFER_SIZE);

            try
            {
                while (this.WebSocket?.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    // await a message
                    var result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    if (result == null)
                    {
                        continue;
                    }

                    if (result.MessageType == WebSocketMessageType.Close || (result.CloseStatus != null && result.CloseStatus.HasValue && result.CloseStatus.Value != WebSocketCloseStatus.Empty))
                    {
                        // stop listening, and return the close status
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
            }
            catch (OperationCanceledException)
            {
                return WebSocketCloseStatus.NormalClosure;
            }
            catch (Exception)
            {
                return WebSocketCloseStatus.InternalServerError;
            }

            return WebSocketCloseStatus.NormalClosure;
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
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
        /// Serializes the value, and sends the message asynchronously.
        /// </summary>
        /// <param name="value">The value to serialize and send.</param>
        public Task SendJsonAsync(object value)
        {
            var json = JsonConvert.SerializeObject(value, this.JsonSettings);
            return this.SendAsync(json);
        }
    }
}
