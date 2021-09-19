namespace Microsoft.Extensions.Logging
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SharpDeck.Logging;

    /// <summary>
    /// Provides extensions for <see cref="ILoggingBuilder"/>.
    /// </summary>
    [Obsolete]
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// Adds loggers capable of logging to the default Stream Deck logs.
        /// </summary>
        /// <param name="loggingBuilder">The logging builder; this instance.</param>
        /// <param name="minimumLevel">The minimum level; otherwise <see cref="LogLevel.Error"/>.</param>
        /// <returns>A logging builder that you can chain additional calls to.</returns>
        public static ILoggingBuilder AddStreamDeck(this ILoggingBuilder loggingBuilder, LogLevel minimumLevel = LogLevel.Error)
        {
            loggingBuilder.Services
                .AddSingleton(services => new StreamDeckLoggerProviderConfiguration(minimumLevel, services.GetService<IConfiguration>()))
                .AddSingleton<ILoggerProvider, StreamDeckLoggerProvider>();

            return loggingBuilder;
        }
    }
}
