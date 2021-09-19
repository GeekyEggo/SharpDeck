namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Microsoft.Extensions.Hosting;
    using SharpDeck.Connectivity;
    using SharpDeck.Connectivity.Net;
    using SharpDeck.DependencyInjection;
    using SharpDeck.Events.Received;
    using SharpDeck.Hosting;
    using SharpDeck.Interactivity;

    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures and adds a new Stream Deck plugin as an <see cref="IHostedService"/>.
        /// </summary>
        /// <param name="services">This instance.</param>
        /// <param name="configurePlugin">The optional configuration to be applied to the plugin.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddStreamDeck(this IServiceCollection services, Action<IPluginBuilder> configurePlugin = default)
        {
            return services
                // Misc
                .AddSingleton<IActivator, SelfContainedServiceProviderActivator>(provider => new SelfContainedServiceProviderActivator(provider))

                // Connection with Stream Deck.
                .AddSingleton(new RegistrationParameters(Environment.GetCommandLineArgs()))
                .AddSingleton<IStreamDeckConnection>(provider => provider.GetRequiredService<StreamDeckWebSocketConnection>())
                .AddSingleton<StreamDeckWebSocketConnection>()

                // Action interactivity.
                .AddSingleton<IDynamicProfileFactory, DynamicProfileFactory>()
                .AddSingleton<IStreamDeckActionRegistry>(provider => provider.GetRequiredService<StreamDeckActionRegistry>())
                .AddSingleton<StreamDeckActionRegistry>(provider =>
                {
                    var builder = ActivatorUtilities.CreateInstance<PluginBuilder>(provider);
                    configurePlugin?.Invoke(builder);

                    var registry = ActivatorUtilities.CreateInstance<StreamDeckActionRegistry>(provider);
                    foreach (var assembly in builder.Assemblies)
                    {
                        registry.RegisterAll(assembly);
                    }

                    return registry;
                })

                // Hosts
                .AddSingleton<IHostedService>(provider => provider.GetRequiredService<StreamDeckActionRegistry>())
                .AddSingleton<IHostedService>(provider => provider.GetRequiredService<StreamDeckWebSocketConnection>());

        }
    }
}
