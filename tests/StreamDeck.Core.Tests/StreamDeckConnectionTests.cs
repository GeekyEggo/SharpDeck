namespace StreamDeck.Tests
{
    using StreamDeck.Net;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeckConnection"/>.
    /// </summary>
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public partial class StreamDeckConnectionTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckConnectionTests"/> class.
        /// </summary>
        public StreamDeckConnectionTests()
            => this.StreamDeckConnection = new StreamDeckConnection(this.RegistrationParameters, this.WebSocketConnection.Object);

        /// <summary>
        /// Gets or sets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; } = new RegistrationParameters("-port", "13", "-pluginUUID", "ABCDEF123456", "-registerEvent", "registerPlugin", "-info", "{}");

        /// <summary>
        /// Gets or sets the Stream Deck connection used to assert tests.
        /// </summary>
        private StreamDeckConnection StreamDeckConnection { get; set; }

        /// <summary>
        /// Gets the mock web socket connection.
        /// </summary>
        private Mock<IWebSocketConnection> WebSocketConnection { get; } = new Mock<IWebSocketConnection>();

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.StreamDeckConnection(RegistrationParameters, IWebSocketConnection?, Microsoft.Extensions.Logging.ILogger{StreamDeckConnection}?)"/>.
        /// </summary>
        [Test]
        public void Construct()
            => Assert.That(this.StreamDeckConnection.Info, Is.EqualTo(this.RegistrationParameters.Info));

        /// <summary>
        /// Connects <see cref="StreamDeckConnection.ConnectAsync(CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task ConnectAsync()
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.ConnectAsync(token);

            // Assert.
            this.WebSocketConnection.Verify(ws => ws.ConnectAsync($"ws://localhost:{this.RegistrationParameters.Port}/", token), Times.Once);
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"event":"{{this.RegistrationParameters.Event}}","uuid":"{{this.RegistrationParameters.PluginUUID}}"}
                    """, token),
                Times.Once);
        }
    }
}
