namespace StreamDeck.Extensions.Hosting
{
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Provides an all-in-one solution for hosting a Stream Deck plugin.
    /// </summary>
    public static class StreamDeckPlugin
    {
        /// <summary>
        /// Creates a <see cref="IHost"/> with Stream Deck plugin connectivity configured.
        /// </summary>
        /// <returns>The configured host.</returns>
        public static IHost Create()
            => CreateBuilder().Build();

        /// <summary>
        /// Creates a <see cref="IHostBuilder"/> with Stream Deck plugin connectivity configured.
        /// </summary>
        /// <returns>The configured host builder.</returns>
        public static IHostBuilder CreateBuilder()
            => new HostBuilder().UsePluginLifetime();
    }
}
