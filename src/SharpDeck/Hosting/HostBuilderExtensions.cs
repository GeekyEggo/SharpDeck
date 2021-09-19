namespace Microsoft.Extensions.Hosting
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using SharpDeck.Hosting;

    /// <summary>
    /// Provides extension methods for <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures the host builder to manage a connection with the Stream Deck. Events received via the Stream Deck are intercepted and handled by the plugin classes.
        /// </summary>
        /// <param name="builder">This instance.</param>
        /// <param name="configurePlugin">The optional configuration to be applied to the plugin.</param>
        /// <returns>The host builder for chaining.</returns>
        public static IHostBuilder UseStreamDeck(this IHostBuilder builder, Action<IPluginBuilder> configurePlugin = default)
            => builder.ConfigureServices((_, services) => services.AddStreamDeck(configurePlugin));
    }
}
