namespace SharpDeck.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Connectivity;
    using SharpDeck.Connectivity.Net;
    using SharpDeck.DependencyInjection;
    using SharpDeck.Events.Received;

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
            return services

                // Connection with Stream Deck.
                .AddSingleton(new RegistrationParameters(Environment.GetCommandLineArgs()))
                .AddSingleton<IStreamDeckConnectionController, StreamDeckWebSocketConnection>()
                .AddSingleton<IStreamDeckConnection>(serviceProvider => serviceProvider.GetRequiredService<IStreamDeckConnectionController>())

                // Action management, and drill-down.
                .AddSingleton<IFactory<StreamDeckAction>, StreamDeckButtonFactory<StreamDeckAction>>(serviceProvider => new StreamDeckButtonFactory<StreamDeckAction>(serviceProvider))
                .AddSingleton<IStreamDeckActionManager, StreamDeckActionManager>()

                // Plug-in.
                .AddSingleton<IStreamDeckPlugin>(serviceProvider =>
                {
                    // Construct the plugin, and configure it.
                    var plugin = new StreamDeckPlugin(
                        serviceProvider.GetRequiredService<IStreamDeckConnectionController>(),
                        serviceProvider.GetRequiredService<IStreamDeckActionManager>());

                    configure?.Invoke(plugin);

                    // Initialize the singleton instance.
                    StreamDeckPlugin.Initialize(plugin);

                    return plugin;
                });
        }
    }
}
