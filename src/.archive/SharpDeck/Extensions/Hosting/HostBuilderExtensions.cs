namespace SharpDeck.Extensions.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SharpDeck.Extensions.DependencyInjection;
    using SharpDeck.Hosting;

    /// <summary>
    /// Provides extension methods for <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Enables Stream Deck plugin support, builds and starts the host, and waits for plugin to disconnect.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        public static void RunStreamDeckPlugin(this IHostBuilder builder)
            => builder.UseStreamDeck().Build().Run();

        /// <summary>
        /// Enables Stream Deck plugin support, builds and starts the host, and waits for plugin to disconnect.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <param name="cancellationToken">The optional cancellation token that can be used to stop the plugin.</param>
        /// <returns>A task that only completes when the plugin disconnects, or the cancellation token is triggered.</returns>
        public static Task RunStreamDeckPluginAsync(this IHostBuilder builder, CancellationToken cancellationToken = default)
            => builder.UseStreamDeck().Build().RunAsync(cancellationToken);

        /// <summary>
        /// Configures the host builder to manage a connection with the Stream Deck. Events received via the Stream Deck are intercepted and handled by the plugin classes.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <returns>The host builder for chaining.</returns>
        private static IHostBuilder UseStreamDeck(this IHostBuilder builder)
            => builder.ConfigureServices(services =>
            {
                services
                    .AddStreamDeck()
                    .AddSingleton<IHostLifetime, StreamDeckPluginHostLifetime>();
            });
    }
}
