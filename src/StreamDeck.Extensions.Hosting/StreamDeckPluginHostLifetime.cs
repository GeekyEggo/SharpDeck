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
        /// <param name="connection">The connection.</param>
        /// <param name="configures">A collection of delegates that configure the <see cref="IStreamDeckConnection"/> prior to connection.</param>
        public StreamDeckPluginHostLifetime(StreamDeckConnection connection, IEnumerable<Action<IStreamDeckConnection>> configures)
        {
            this.Connection = connection;
            foreach (var configure in configures)
            {
                configure(connection);
            }
        }

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        private StreamDeckConnection Connection { get; }

        /// <inheritdoc/>
        public Task WaitForStartAsync(CancellationToken cancellationToken)
            => this.Connection.ConnectAsync(cancellationToken);

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken)
            => await this.Connection.DisposeAsync();
    }
}
