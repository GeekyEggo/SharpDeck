namespace SharpDeck.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides the hosted service that manages the connection between the plugin and the Stream Deck.
    /// </summary>
    internal class StreamDeckPluginService : IHostedService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckPluginService" /> class.
        /// </summary>
        /// <param name="connection">The connection to the Stream Deck.</param>
        /// <param name="actionRegistry">The action registry.</param>
        public StreamDeckPluginService(IStreamDeckConnectionController connection, IStreamDeckActionRegistry actionRegistry, ILogger<StreamDeckPluginService> logger)
        {
            this.Logger = logger;

            this.ActionRegistry = actionRegistry;
            this.Connection = connection;

            actionRegistry.IsEnabled = true;
        }

        /// <summary>
        /// Gets the action registry.
        /// </summary>
        public IStreamDeckActionRegistry ActionRegistry { get; }

        /// <summary>
        /// Gets the connection to the Stream Deck.
        /// </summary>
        private IStreamDeckConnectionController Connection { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger<StreamDeckPluginService> Logger { get; }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.Logger.LogTrace("Plugin starting.");
            return this.Connection.ConnectAsync(cancellationToken);
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogCritical("Plugin stopping.");

            this.ActionRegistry.IsEnabled = false;
            return this.Connection.DisconnectAsync(cancellationToken);
        }
    }
}
