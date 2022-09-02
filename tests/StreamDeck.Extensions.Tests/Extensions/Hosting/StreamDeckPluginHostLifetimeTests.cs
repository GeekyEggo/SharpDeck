namespace StreamDeck.Tests.Extensions.Hosting.Tests
{
    using System.Threading.Tasks;
    using Moq;
    using StreamDeck.Extensions.Hosting;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeckPluginHostLifetime"/>.
    /// </summary>
    [TestFixture]
    public class StreamDeckPluginHostLifetimeTests
    {
        /// <summary>
        /// Asserts <see cref="StreamDeckPluginHostLifetime.WaitForStartAsync(CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task WaitForStartAsync()
        {
            // Arrange.
            var connector = new Mock<IStreamDeckConnectionManager>();
            var hostLifetime = new StreamDeckPluginHostLifetime(connector.Object);
            var cancellationToken = new CancellationToken();

            // Act.
            await hostLifetime.WaitForStartAsync(cancellationToken);

            // Assert.
            connector.Verify(c => c.ConnectAsync(cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckPluginHostLifetime.StopAsync(CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task StopAsync()
        {
            // Arrange.
            var connector = new Mock<IStreamDeckConnectionManager>();
            var hostLifetime = new StreamDeckPluginHostLifetime(connector.Object);

            // Act.
            await hostLifetime.StopAsync(default);

            // Assert.
            connector.Verify(c => c.DisposeAsync(), Times.Once);
        }
    }
}
