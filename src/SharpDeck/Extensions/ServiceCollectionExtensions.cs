﻿namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using SharpDeck;
    using SharpDeck.Connectivity;
    using SharpDeck.Connectivity.Net;
    using SharpDeck.DependencyInjection;
    using SharpDeck.Events.Received;
    using SharpDeck.Interactivity;

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
                // Misc
                .AddSingleton<IActivator, SelfContainedServiceProviderActivator>(serviceProvider => new SelfContainedServiceProviderActivator(serviceProvider))

                // Connection with Stream Deck.
                .AddSingleton(new RegistrationParameters(Environment.GetCommandLineArgs()))
                .AddSingleton<IStreamDeckConnectionController, StreamDeckWebSocketConnection>()
                .AddSingleton<IStreamDeckConnection>(serviceProvider => serviceProvider.GetRequiredService<IStreamDeckConnectionController>())

                // Action interactivity.
                .AddSingleton<IDrillDownFactory, DrillDownFactory>()
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
