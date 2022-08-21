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
    }
}
