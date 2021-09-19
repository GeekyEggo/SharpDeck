namespace SharpDeck.Logging
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides configuration for <see cref="StreamDeckLoggerProvider"/>.
    /// </summary>
    [Obsolete]
    internal class StreamDeckLoggerProviderConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckLoggerProviderConfiguration" /> class.
        /// </summary>
        /// <param name="minimumLevel">The minimum level.</param>
        /// <param name="configuration">The optional configuration.</param>
        public StreamDeckLoggerProviderConfiguration(LogLevel minimumLevel = LogLevel.Error, IConfiguration configuration = null)
        {
            this.MinimumLevel = minimumLevel;

            foreach (var child in configuration?.GetSection("Logging:StreamDeck:LogLevel")?.GetChildren())
            {
                // Validate the string value of the log level.
                if (!Enum.TryParse<LogLevel>(child.Value, out var logLevel))
                {
                    throw new InvalidCastException($"Failed to parse log level \"{child.Value}\".");
                }

                if (child.Key == "*")
                {
                    // Default wildcard.
                    this.MinimumLevel = logLevel;
                }
                else if (this.CategoryRules.ContainsKey(child.Key))
                {
                    // Overwrite the existing value.
                    this.CategoryRules[child.Key] = logLevel;
                }
                else
                {
                    this.CategoryRules.Add(child.Key, logLevel);
                }
            }
        }

        /// <summary>
        /// Gets the minimum log levels, grouped by their category.
        /// </summary>
        public Dictionary<string, LogLevel> CategoryRules { get; } = new Dictionary<string, LogLevel>();

        /// <summary>
        /// Gets the minimum log level.
        /// </summary>
        public LogLevel MinimumLevel { get; }
    }
}
