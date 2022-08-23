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
        /// Adds a delegate for configuring the provided <see cref="IStreamDeckConnection"/> prior to connecting to the Stream Deck.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder" /> to configure.</param>
        /// <param name="configurePlugin">The delegate that configures the <see cref="IStreamDeckConnection"/>.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder ConfigurePlugin(this IHostBuilder builder, Action<IStreamDeckConnection> configurePlugin)
            => builder.ConfigureServices(services => services.AddSingleton(configurePlugin));
    }
}
