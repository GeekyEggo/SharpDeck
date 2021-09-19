namespace SharpDeck.Logging
{
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides an implementation of <see cref="ILoggerProvider"/> that utilizes the <see cref="IStreamDeckConnection.LogMessageAsync(string, CancellationToken)"/> for logging.
    /// </summary>
    [ProviderAlias("StreamDeck")]
    internal sealed class StreamDeckLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckLoggerProvider" /> class.
        /// </summary>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="configuration">The optional configuration.</param>
        public StreamDeckLoggerProvider(IStreamDeckConnection connection, StreamDeckLoggerProviderConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Connection = connection;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        private StreamDeckLoggerProviderConfiguration Configuration { get; }

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Creates a new <see cref="ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The instance of <see cref="ILogger" /> that was created.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            // When there is a category rule defined, the Microsoft.Extensions.Logging will take over.
            var logLevel = this.Configuration.CategoryRules.TryGetValue(categoryName, out var definedLogLevel) ? definedLogLevel : this.Configuration.MinimumLevel;
            return new StreamDeckLogger(categoryName, this.Connection, logLevel);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { /* Nothing to do. */ }
    }
}