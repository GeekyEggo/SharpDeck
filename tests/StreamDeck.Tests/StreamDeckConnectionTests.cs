namespace StreamDeck.Tests
{
    using Moq;
    using StreamDeck.Net;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeckConnection"/>.
    /// </summary>
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public partial class StreamDeckConnectionTests
    {
        /// <summary>
        /// Gets the mock web socket connection.
        /// </summary>
        private Mock<IWebSocketConnection> WebSocketConnection { get; } = new Mock<IWebSocketConnection>();

        /// <summary>
        /// Gets or sets the Stream Deck connection used to assert tests.
        /// </summary>
        private StreamDeckConnection StreamDeckConnection { get; set; }

        /// <summary>
        /// Provides per-test set-up.
        /// </summary>
        [SetUp]
        public void SetUp()
            => this.StreamDeckConnection = new StreamDeckConnection(new RegistrationParameters("-port", "13", "-pluginUUID", "ABCDEF123456", "-registerEvent", "registerPlugin", "-info", "{}"), this.WebSocketConnection.Object);
    }
}
