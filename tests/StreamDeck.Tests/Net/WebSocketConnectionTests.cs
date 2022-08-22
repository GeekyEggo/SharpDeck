namespace StreamDeck.Tests.Net
{
    using StreamDeck.Net;

    /// <summary>
    /// Provides assertions for <see cref="WebSocketConnection"/>.
    /// </summary>
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class WebSocketConnectionTests
    {
        /// <summary>
        /// The URI of the web socket server.
        /// </summary>
        private const string Uri = "ws://127.0.0.1:8181";

        /// <summary>
        /// Gets the web socket server.
        /// </summary>
        private Fleck.WebSocketServer Server { get; } = new Fleck.WebSocketServer(Uri);

        /// <summary>
        /// Gets the web socket client.
        /// </summary>
        private WebSocketConnection Client { get; } = new WebSocketConnection();

        /// <summary>
        /// Provides per-test tear down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.Client.Dispose();
            this.Server.Dispose();
        }

        /// <summary>
        /// Asserts <see cref="WebSocketConnection.ConnectAsync(string, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task ConnectAsync()
        {
            // Arrange.
            var tcs = new TaskCompletionSource<Fleck.IWebSocketConnection>();
            this.Server.Start(conn => conn.OnOpen = () => tcs.TrySetResult(conn));

            // Act.
            await this.Client.ConnectAsync(Uri);

            // Assert.
            await tcs.Task;
            Assert.Pass();
        }

        /// <summary>
        /// Asserts <see cref="WebSocketConnection.DisconnectAsync()"/>.
        /// </summary>
        [Test]
        public async Task DisconnectAsync()
        {
            // Arrange.
            var tcs = new TaskCompletionSource<Fleck.IWebSocketConnection>();
            this.Server.Start(conn => conn.OnClose = () => tcs.TrySetResult(conn));

            await this.Client.ConnectAsync(Uri);

            // Act.
            await this.Client.DisconnectAsync();

            // Assert.
            await tcs.Task;
            Assert.Pass();
        }

        /// <summary>
        /// Asserts <see cref="WebSocketConnection.DisposeAsync()"/>.
        /// </summary>
        [Test]
        public async Task DisposeAsync()
        {
            // Arrange.
            var tcs = new TaskCompletionSource<Fleck.IWebSocketConnection>();
            this.Server.Start(conn => conn.OnClose = () => tcs.TrySetResult(conn));

            await this.Client.ConnectAsync(Uri);

            // Act.
            await this.Client.DisposeAsync();

            // Assert.
            await tcs.Task;
            Assert.Pass();
        }

        /// <summary>
        /// Asserts <see cref="WebSocketConnection.MessageReceived"/>.
        /// </summary>
        [Test]
        public async Task MessageReceived()
        {
            // Arrange.
            var tcs = new TaskCompletionSource<string>();

            this.Server.Start(conn => conn.OnMessage = _ => conn.Send("Pong"));
            await this.Client.ConnectAsync(Uri);

            this.Client.MessageReceived += (sender, args) => tcs.TrySetResult(args.Message);

            // Act
            await this.Client.SendAsync("Ping");

            // Assert.
            var response = await tcs.Task;
            Assert.That(response, Is.EqualTo("Pong"));
        }

        /// <summary>
        /// Asserts <see cref="WebSocketConnection.SendAsync(object, System.Text.Json.JsonSerializerOptions?, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task SendAsync()
        {
            // Arrange.
            var tcs = new TaskCompletionSource<string>();
            this.Server.Start(conn => conn.OnMessage = (msg) => tcs.TrySetResult(msg));

            await this.Client.ConnectAsync(Uri);

            // Act.
            await this.Client.SendAsync("Lorem ipsum");
            var msg = await tcs.Task;

            // Assert.
            Assert.That(msg, Is.EqualTo("Lorem ipsum"));
        }

        /// <summary>
        /// Asserts <see cref="WebSocketConnection.WaitForDisconnectAsync(CancellationToken)()"/>.
        /// </summary>
        [Test]
        public async Task WaitForDisconnectAsync()
        {
            // Arrange.
            var didClose = false;
            this.Server.Start(conn => conn.OnClose = () => didClose = true);

            // Act.
            await this.Client.ConnectAsync(Uri);
            _ = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(500);
                await this.Client.DisconnectAsync();
            }, TaskCreationOptions.RunContinuationsAsynchronously);

            // Assert.
            await this.Client.WaitForDisconnectAsync();
            Assert.That(didClose, Is.True);
        }
    }
}
