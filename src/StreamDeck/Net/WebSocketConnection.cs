namespace StreamDeck.Net
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a light-weight wrapper for <see cref="ClientWebSocket"/>.
    /// </summary>
    internal sealed class WebSocketConnection : IWebSocketConnection
    {
        /// <summary>
        /// The buffer size.
        /// </summary>
        private const int BUFFER_SIZE = 1024 * 1024; // 1KiB.

        /// <summary>
        /// The process synchronize root.
        /// </summary>
        private readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<WebSocketMessageEventArgs>? MessageReceived;

        /// <summary>
        /// Gets the connection task completion source.
        /// </summary>
        private TaskCompletionSource<bool> ConnectionTaskCompletionSource { get; } = new TaskCompletionSource<bool>();

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        private Encoding Encoding { get; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        private ClientWebSocket? WebSocket { get; set; }

        /// <inheritdoc/>
        public async Task ConnectAsync(string uri, CancellationToken cancellationToken = default)
        {
            try
            {
                await this._syncRoot.WaitAsync(cancellationToken);

                if (this.WebSocket == null)
                {
                    // Connect the web socket.
                    this.WebSocket = new ClientWebSocket();
                    await this.WebSocket.ConnectAsync(new Uri(uri), cancellationToken);

                    // Asynchronously listen for messages.
                    _ = Task.Factory.StartNew(
                        async () =>
                        {
                            await this.ReceiveAsync(cancellationToken);
                            this.ConnectionTaskCompletionSource.TrySetResult(true);
                        },
                        cancellationToken,
                        TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously,
                        TaskScheduler.Default);
                }
            }
            finally
            {
                this._syncRoot.Release();
            }
        }

        /// <inheritdoc/>
        public async Task DisconnectAsync()
        {
            try
            {
                await this._syncRoot.WaitAsync();

                if (this.WebSocket != null)
                {
                    var socket = this.WebSocket;
                    this.WebSocket = null;

                    if (socket.State == WebSocketState.Open)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
                    }

                    socket.Dispose();
                    this.ConnectionTaskCompletionSource?.TrySetResult(true);
                }
            }
            finally
            {
                this._syncRoot.Release();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _ = this.DisconnectAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await this.DisconnectAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async Task SendAsync(string message, CancellationToken cancellationToken = default)
        {
            if (this.WebSocket?.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("The web socket connection is not in an open state.");
            }

            try
            {
                await this._syncRoot.WaitAsync(cancellationToken);

                var buffer = this.Encoding.GetBytes(message);
                await this.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                this._syncRoot.Release();
            }
        }

        /// <inheritdoc/>
        public Task WaitForDisconnectAsync(CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            return Task.WhenAny(tcs.Task, this.ConnectionTaskCompletionSource.Task);
        }

        /// <summary>
        /// Receive data as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<WebSocketCloseStatus> ReceiveAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[BUFFER_SIZE];
            var textBuffer = new StringBuilder(BUFFER_SIZE);

            try
            {
                while (this.WebSocket?.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    // Await a message.
                    var result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    if (result == null)
                    {
                        continue;
                    }

                    // Stop listening, and return the close status.
                    if (result.MessageType == WebSocketMessageType.Close
                        || (result.CloseStatus != null && result.CloseStatus != WebSocketCloseStatus.Empty))
                    {
                        return result.CloseStatus.GetValueOrDefault();
                    }

                    // Otherwise, build the text buffer until we have received the entire message.
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
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
    }
}
