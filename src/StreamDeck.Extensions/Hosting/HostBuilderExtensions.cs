namespace StreamDeck.Extensions.Hosting
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using StreamDeck.Extensions.Routing;

    /// <summary>
    /// Provides extension methods for <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Upon starting, the host connects to the Stream Deck and registers the plugin.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder" /> to configure.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder UsePluginLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((ctx, services) =>
            {
                // Connection.
                services.TryAddSingleton<StreamDeckConnection>();
                services.TryAddSingleton<IStreamDeckConnection>(s => s.GetRequiredService<StreamDeckConnection>());
                services.TryAddSingleton<IStreamDeckConnectionManager>(s => s.GetRequiredService<StreamDeckConnection>());

                // Routing.
                services.TryAddSingleton<IActionFactory, ActivatorUtilitiesActionFactory>();
                services.TryAddSingleton<IDispatcher, AsyncDispatcher>();
                services.TryAddSingleton<ActionRouter>();

                // Hosting.
                services.AddSingleton<IHostLifetime, StreamDeckPluginHostLifetime>();
            });
        }
    }
}
