namespace SharpDeck.Extensions.Hosting
{
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;

    /// <summary>
    /// Provides an all-in-one solution for hosting a Stream Deck plugin including logging, configuration, and connectivity setup.
    /// </summary>
    public static class StreamDeckPluginHost
    {
        /// <summary>
        /// Creates a <see cref="IHostBuilder"/> with configuration, NLog logging, and Stream Deck connectivity all configured.
        /// </summary>
        /// <returns>The configured host builder.</returns>
        public static IHostBuilder CreateDefaultBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("SharpDeck.Extensions.Hosting.appsettings.json"))
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                })
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder
                        .ClearProviders()
                        .AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
                        .AddNLog(new NLogLoggingConfiguration(hostingContext.Configuration.GetSection("NLog")));
                });
        }

        /// <summary>
        /// Runs the Stream Deck plugin.
        /// </summary>
        public static void Run()
            => CreateDefaultBuilder().RunStreamDeckPlugin();

        /// <summary>
        /// Runs the Stream Deck plugin asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token that can be used to stop the plugin.</param>
        /// <returns>The task of running the Stream Deck plugin.</returns>
        public static Task RunAsync(CancellationToken cancellationToken = default)
            => CreateDefaultBuilder().RunStreamDeckPluginAsync(cancellationToken);
    }
}
