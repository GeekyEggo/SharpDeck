namespace StreamDeck.Tests.Routing
{
    using StreamDeck.Events;
    using StreamDeck.Routing;

    /// <summary>
    /// Provides assertions for <see cref="AsyncEventDispatcher"/>.
    /// </summary>
    [TestFixture]
    public class AsyncEventDispatcherTests
    {
        /// <summary>
        /// Asserts that <see cref="AsyncEventDispatcher.Invoke{TArgs}(Func{TArgs, Task}, TArgs)"/> invokes the delegate with the given arguments.
        /// </summary>
        [Test]
        public async Task Invoke()
        {
            // Arrange.
            var connection = new Mock<IStreamDeckConnection>();
            var dispatcher = new AsyncEventDispatcher(connection.Object);

            ActionEventArgs? actualArgs = null;
            var expectedArgs = new ActionEventArgs("event", "action", "context", "device");

            // Act.
            dispatcher.Invoke(
                args => Task.FromResult(actualArgs = args),
                expectedArgs);

            // Assert.
            await dispatcher.DisposeAsync();
            Assert.That(actualArgs, Is.Not.Null);
            Assert.That(actualArgs, Is.SameAs(expectedArgs));
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
            var dispatcher = new AsyncEventDispatcher(connection.Object);

            // Act.
            dispatcher.Invoke(
                a => throw new NotImplementedException(message),
                new ActionEventArgs("event", "action", context, "device"));

            // Assert.
            await dispatcher.DisposeAsync();
            connection.Verify(c => c.LogMessageAsync(message, It.IsAny<CancellationToken>()));
            connection.Verify(c => c.ShowAlertAsync(context, It.IsAny<CancellationToken>()));
        }
    }
}
