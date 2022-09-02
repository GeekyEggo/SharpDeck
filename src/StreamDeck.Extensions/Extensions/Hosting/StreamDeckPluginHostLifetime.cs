namespace StreamDeck.Extensions.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using StreamDeck;

    /// <summary>
    /// Provides a <see cref="IHostLifetime"/> that allows for <see cref="IHostedService"/> to start when a connection has been established with the Stream Deck.
    /// </summary>
    internal class StreamDeckPluginHostLifetime : IHostLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckPluginHostLifetime"/> class.
        /// </summary>
        /// <param name="connectionManager">The connector manager used to establish a connection with the Stream Deck.</param>
        public StreamDeckPluginHostLifetime(IStreamDeckConnectionManager connectionManager)
            => this.ConnectionManager = connectionManager;

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        private IStreamDeckConnectionManager ConnectionManager { get; }

        /// <inheritdoc/>
        public Task WaitForStartAsync(CancellationToken cancellationToken)
            => this.ConnectionManager.ConnectAsync(cancellationToken);

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken)
            => await this.ConnectionManager.DisposeAsync();
    }
}
