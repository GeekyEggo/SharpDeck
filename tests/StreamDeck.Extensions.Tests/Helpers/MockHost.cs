namespace StreamDeck.Extensions.Tests.Helpers
{
    using Microsoft.Extensions.Hosting;
    using StreamDeck.Extensions.Routing;

    /// <summary>
    /// Provides a fully-configured mock <see cref="IHost"/>.
    /// </summary>
    internal class MockHost : Mock<IHost>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockHost"/> class.
        /// </summary>
        public MockHost()
            : base()
        {
            this.SetupGet(h => h.Services)
                .Returns(() => this.ServiceProvider.Object);

            this.Dispatcher
                .Setup(d => d.Invoke(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Callback<Func<Task>, string>((action, context) => action().Wait());

            this.ServiceProvider.Setup(s => s.GetService(typeof(IDispatcher))).Returns(this.Dispatcher.Object);
            this.ServiceProvider.Setup(s => s.GetService(typeof(IService))).Returns(this.Service);
            this.ServiceProvider.Setup(s => s.GetService(typeof(IStreamDeckConnection))).Returns(this.Connection.Object);
        }

        /// <summary>
        /// Gets the mock <see cref="IDispatcher"/>.
        /// </summary>
        public Mock<IDispatcher> Dispatcher { get; } = new Mock<IDispatcher>();

        /// <summary>
        /// Gets the mock <see cref="IStreamDeckConnection"/>.
        /// </summary>
        public Mock<IStreamDeckConnection> Connection { get; } = new Mock<IStreamDeckConnection>();

        /// <summary>
        /// Gets the <see cref="IService"/>, used to assert resolution.
        /// </summary>
        public IService Service { get; } = new IService();

        /// <summary>
        /// Gets the mock <see cref="IServiceProvider"/> that represents the <see cref="IHost.Services"/>.
        /// </summary>
        public Mock<IServiceProvider> ServiceProvider { get; } = new Mock<IServiceProvider>();
    }
}
