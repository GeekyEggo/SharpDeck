namespace SharpDeck
{
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Connectivity;
    using SharpDeck.Hosting;

    /// <summary>
    /// Provides a host for a Stream Deck plugin.
    /// </summary>
    internal class PluginHost : IPluginHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginHost"/> class.
        /// </summary>
        /// <param name="connection">The connection to the Stream Deck.</param>
        public PluginHost(IStreamDeckConnectionController connection)
            => this.Connection = connection;

        /// <summary>
        /// Gets the connection to the Stream Deck.
        /// </summary>
        private IStreamDeckConnectionController Connection { get; }

        /// <summary>
        /// Starts the plugin host.
        /// </summary>
        public void Start()
            => Task.WaitAll(this.StartAsync());

        /// <summary>
        /// Starts the plugin host asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of connecting and maintaining the connection with the Stream Deck.</returns>
        public Task StartAsync(CancellationToken cancellationToken = default)
            => this.Connection.ConnectAsync(cancellationToken);
    }
}
