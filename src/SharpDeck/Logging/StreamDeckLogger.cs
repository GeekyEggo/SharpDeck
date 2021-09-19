namespace SharpDeck.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Connectivity;
    using SharpDeck.Extensions;

    /// <summary>
    /// Provides an implementation of <see cref="ILogger"/> that utilizes <see cref="IStreamDeckConnection.LogMessageAsync(string, System.Threading.CancellationToken)"/>.
    /// </summary>
    internal sealed class StreamDeckLogger : ILogger
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Gets the text that represents the <see cref="LogLevel"/>.
        /// </summary>
        private static readonly IReadOnlyDictionary<LogLevel, string> LOG_LEVEL_TEXT = new Dictionary<LogLevel, string>
        {
            { LogLevel.Trace, "TRACE" },
            { LogLevel.Debug, "DEBUG" },
            { LogLevel.Information, "INFO" },
            { LogLevel.Warning, "WARN" },
            { LogLevel.Error, "ERROR" },
            { LogLevel.Critical, "FATAL" },
            { LogLevel.None, string.Empty },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckLogger"/> class.
        /// </summary>
        /// <param name="categoryName">The name of the category the logger is associated with.</param>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="minimumLevel">The minimum log level.</param>
        public StreamDeckLogger(string categoryName, IStreamDeckConnection connection, LogLevel minimumLevel)
        {
            this.CategoryName = categoryName;
            this.Connection = connection;
            this.MinimumLevel = minimumLevel;
        }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        private string CategoryName { get; }

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the minimum log level.
        /// </summary>
        private LogLevel MinimumLevel { get; }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>A disposable object that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state)
            => EmptyScope.Instance;

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns><see langword="true" /> if enabled; <see langword="false" /> otherwise.</returns>
        public bool IsEnabled(LogLevel logLevel)
            => logLevel >= this.MinimumLevel;

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <see cref="T:System.String" /> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            var msg = new StringBuilder();
            if (LOG_LEVEL_TEXT.TryGetValue(logLevel, out var logLevelText))
            {
                msg.Append($"[{logLevelText}] ");
            }

            if (!string.IsNullOrWhiteSpace(this.CategoryName))
            {
                msg.Append($"{this.CategoryName} - ");
            }

            msg.Append(formatter(state, exception));
            this.LogAsync(msg.ToString()).ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a log entry asynchronously.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private async Task LogAsync(string message)
        {
            using (await _syncRoot.LockAsync())
            {
                await this.Connection.LogMessageAsync(message);
            }
        }

        /// <summary>
        /// An empty scope.
        /// </summary>
        private sealed class EmptyScope : IDisposable
        {
            /// <summary>
            /// The default empty scope.
            /// </summary>
            public static readonly EmptyScope Instance = new EmptyScope();

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() { /* Nothing to do. */ }
        }
    }
}
