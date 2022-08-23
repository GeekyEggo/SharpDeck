namespace StreamDeck.Extensions.Hosting
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using StreamDeck;

    /// <summary>
    /// Provides an all-in-one solution for hosting a Stream Deck plugin.
    /// </summary>
    public static class StreamDeckPlugin
    {
        /// <summary>
        /// Creates a <see cref="IHostBuilder"/> with Stream Deck plugin connectivity configured.
        /// </summary>
        /// <returns>The configured host builder.</returns>
        public static IHostBuilder Create()
            => Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services
                        .AddSingleton<StreamDeckConnection>()
                        .AddSingleton<IStreamDeckConnection>(s => s.GetRequiredService<StreamDeckConnection>())
                        .AddSingleton<IHostLifetime, StreamDeckPluginHostLifetime>();
                });
    }
}
