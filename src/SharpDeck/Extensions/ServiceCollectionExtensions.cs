namespace SharpDeck.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using SharpDeck.Connectivity.Net;

    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the <see cref="IStreamDeckPlugin"/> and <see cref="IStreamDeckConnection"/> with the service collection, after configuring the plugin.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">The optional configuration to be applied to the <see cref="IStreamDeckPlugin"/> prior to registration.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddStreamDeckPlugin(this IServiceCollection services, Action<IStreamDeckPlugin> configure = null)
        {
            var connectionController = new StreamDeckWebSocketConnection();

            return services
                .AddSingleton<IStreamDeckConnection>(connectionController)
                .AddSingleton<IStreamDeckPlugin>(serviceProvider =>
                {
                    // Construct the plugin, and configure it.
                    var plugin = new StreamDeckPlugin(connectionController, serviceProvider);
                    configure?.Invoke(plugin);

                    // Initialize the singleton instance.
                    StreamDeckPlugin.Initialize(plugin);

                    return plugin;
                });
        }
    }
}
