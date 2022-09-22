namespace StreamDeck.Tests.Routing
{
    using StreamDeck.Events;
    using StreamDeck.Routing;

    /// <summary>
    /// Provides assertions for <see cref="AsyncDispatcher"/>.
    /// </summary>
    [TestFixture]
    public class AsyncDispatcherTests
    {
        /// <summary>
        /// Asserts that <see cref="AsyncDispatcher.Invoke(Func{Task}, string)"/> invokes the delegate with the given arguments.
        /// </summary>
        [Test]
        public async Task Invoke()
        {
            // Arrange.
            var connection = new Mock<IStreamDeckConnection>();
            var dispatcher = new AsyncDispatcher(connection.Object);

            var isInvoked = false;

            // Act.
            dispatcher.Invoke(() => Task.FromResult(isInvoked = true));

            // Assert.
            await dispatcher.DisposeAsync();
            Assert.That(isInvoked, Is.True);
        }

        /// <summary>
        /// Asserts <see cref="IStreamDeckConnection.LogMessageAsync(string, CancellationToken)"/> is invoked when an exception is thrown.
        /// </summary>
        [Test]
        public async Task LogAndDoNotShowAlert()
        {
            // Arrange.
            const string context = "ABC123";
            const string message = "Hello world, this is a test";

            var connection = new Mock<IStreamDeckConnection>();
            var dispatcher = new AsyncDispatcher(connection.Object);

            // Act.
            dispatcher.Invoke(() => throw new NotImplementedException(message));

            // Assert.
            await dispatcher.DisposeAsync();
            connection.Verify(c => c.LogMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
            connection.Verify(c => c.ShowAlertAsync(context, It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Asserts <see cref="IStreamDeckConnection.LogMessageAsync(string, CancellationToken)"/> and <see cref="IStreamDeckConnection.ShowAlertAsync(string, CancellationToken)"/> are invoked when an exception is thrown.
        /// </summary>
        [Test]
        public async Task LogAndShowAlert()
        {
            // Arrange.
            const string context = "ABC123";
            const string message = "Hello world, this is a test";

            var connection = new Mock<IStreamDeckConnection>();
            var dispatcher = new AsyncDispatcher(connection.Object);

            // Act.
            dispatcher.Invoke(
                () => throw new NotImplementedException(message),
                context);

            // Assert.
            await dispatcher.DisposeAsync();
            connection.Verify(c => c.LogMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
            connection.Verify(c => c.ShowAlertAsync(context, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
