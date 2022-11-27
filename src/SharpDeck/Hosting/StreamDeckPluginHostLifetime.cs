namespace SharpDeck.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using SharpDeck.Connectivity;
    using SharpDeck.Connectivity.Net;

    /// <summary>
    /// Provides a <see cref="IHostLifetime"/> that allows for <see cref="IHostedService"/> to start when a connection has been established with the Stream Deck.
    /// </summary>
    internal class StreamDeckPluginHostLifetime : IHostLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckPluginHostLifetime"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="actionRegistry">The action registry.</param>
        public StreamDeckPluginHostLifetime(StreamDeckWebSocketConnection connection, IStreamDeckActionRegistry actionRegistry)
        {
            if (actionRegistry is StreamDeckActionRegistry registry)
            {
                registry.IsEnabled = true;
            }
            this.Connection = connection;
        }

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        private StreamDeckWebSocketConnection Connection { get; }

        /// <inheritdoc/>
        public Task WaitForStartAsync(CancellationToken cancellationToken)
            => this.Connection.ConnectAsync(cancellationToken);

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
            => this.Connection.DisconnectAsync();
    }
}
