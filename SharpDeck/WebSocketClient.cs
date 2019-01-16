using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpDeck
{
    //https://www.c-sharpcorner.com/UploadFile/bhushanbhure/websocket-server-using-httplistener-and-client-with-client/
    public class WebSocketClient : IDisposable
    {
        public WebSocketClient(string uri, int port)
        {
            this.Uri = new Uri(uri);
            this.WebSocket = new ClientWebSocket();
            this.WebSocket.Options.AddSubProtocol(port.ToString());
        }

        public EventHandler<string> OnMessage;

        private CancellationTokenSource CancellationTokenSource { get; set; }
        private Encoding Encoding { get; set; } = Encoding.UTF8;
        private Uri Uri { get; }
        private ClientWebSocket WebSocket { get; set; }

        public async Task ConnectAsync()
        {
            if (this.WebSocket.State == WebSocketState.Open)
            {
                return;
            }

            await this.WebSocket.ConnectAsync(this.Uri, this.CancellationTokenSource.Token);
            await this.ReceiveAsync();
        }

        public Task SendAsync(string msg)
            => this.WebSocket.SendAsync(new ArraySegment<byte>(this.Encoding.GetBytes(msg)), WebSocketMessageType.Binary, false, this.CancellationTokenSource.Token);

        private async Task ReceiveAsync()
        {
            var buffer = new byte[1024];
            while (this.WebSocket.State == WebSocketState.Open)
            {
                var result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), this.CancellationTokenSource.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await this.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, this.CancellationTokenSource.Token);
                }
                else
                {
                    this.OnMessage?.Invoke(this, this.Encoding.GetString(buffer).TrimEnd('\0'));
                }
            }
        }

        public void Dispose()
        {
            this.CancellationTokenSource.Cancel();

            this.WebSocket?.Abort();
            this.WebSocket?.Dispose();
            this.WebSocket = null;
        }
    }
}
