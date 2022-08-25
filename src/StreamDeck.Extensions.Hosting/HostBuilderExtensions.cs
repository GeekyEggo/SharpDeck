namespace StreamDeck.Extensions.Hosting
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
                const string isConnectionConfigured = "streamdeck:isConnectionConfigured";
                if (!ctx.Properties.ContainsKey(isConnectionConfigured))
                {
                    services
                        .AddSingleton<StreamDeckConnection>()
                        .AddSingleton<IStreamDeckConnection>(s => s.GetRequiredService<StreamDeckConnection>())
                        .AddSingleton<IStreamDeckConnector>(s => s.GetRequiredService<StreamDeckConnection>());

                    ctx.Properties.Add(isConnectionConfigured, true);
                }

                services.AddSingleton<IHostLifetime, StreamDeckPluginHostLifetime>();
            });
        }
    }
}
