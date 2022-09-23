namespace StreamDeck.Tests.Extensions.Hosting
{
    using Microsoft.Extensions.Hosting;
    using StreamDeck.Events;
    using StreamDeck.Extensions.Hosting;
    using StreamDeck.Routing;
    using StreamDeck.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="HostExtensions"/>.
    /// </summary>
    [TestFixture]
    public class HostExtensionsTests
    {
        /// <summary>
        /// Asserts the <see cref="HostExtensions.MapKeyDown(IHost, Delegate)"/> correctly resolves all arguments, the delegate is invoked, and it is invoked on the dispatcher.
        /// </summary>
        [Test]
        public void MapKeyDown()
        {
            // Arrange.
            var wasInvoked = false;

            var (host, serviceProvider, connection, dispatcher, service) = CreateTestCase();
            var eventArgs = EventArgsBuilder.CreateKeyEventArgs();

            // Act.
            host.MapKeyDown(Handler);
            connection.Raise(c => c.KeyDown += null, connection.Object, eventArgs);

            // Assert.
            Assert.That(wasInvoked, Is.True);
            dispatcher.Verify(d => d.Invoke(It.IsAny<Func<Task>>(), eventArgs.Context), Times.Once);

            void Handler(IStreamDeckConnection sender, ActionEventArgs<KeyPayload> args, IService resolvedService)
            {
                wasInvoked = true;
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(connection.Object));
                    Assert.That(args, Is.EqualTo(eventArgs));
                    Assert.That(resolvedService, Is.EqualTo(service));
                });
            }
        }

        /// <summary>
        /// Creates typical objects required to assert an extension method on <see cref="IHost"/>.
        /// </summary>
        /// <returns>The supporting objects for a test case.</returns>
        private static (IHost Host, Mock<IServiceProvider> ServiceProvider, Mock<IStreamDeckConnection> Connection, Mock<IDispatcher> Dispatcher, IService Service) CreateTestCase()
        {
            var host = new Mock<IHost>();
            var serviceProvider = new Mock<IServiceProvider>();
            var connection = new Mock<IStreamDeckConnection>();
            var service = new Mock<IService>();

            var dispatcher = new Mock<IDispatcher>();
            dispatcher
                .Setup(d => d.Invoke(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Callback<Func<Task>, string>((action, context) => action().Wait());

            host.SetupGet(h => h.Services).Returns(() => serviceProvider.Object);
            serviceProvider.Setup(s => s.GetService(typeof(IDispatcher))).Returns(dispatcher.Object);
            serviceProvider.Setup(s => s.GetService(typeof(IService))).Returns(service.Object);
            serviceProvider.Setup(s => s.GetService(typeof(IStreamDeckConnection))).Returns(connection.Object);

            return (host.Object, serviceProvider, connection, dispatcher, service.Object);
        }

        /// <summary>
        /// A mock service.
        /// </summary>
        public interface IService { }
    }
}
