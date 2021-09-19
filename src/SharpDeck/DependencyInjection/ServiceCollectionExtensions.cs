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
            var registrationParameters = new RegistrationParameters(Environment.GetCommandLineArgs());
            var connection = new StreamDeckWebSocketConnection(registrationParameters);

            var pluginBuilder = new PluginBuilder(connection, registrationParameters, services);
            configurePlugin?.Invoke(pluginBuilder);

            return services
                // Misc
                .AddSingleton<IActivator, SelfContainedServiceProviderActivator>(provider => new SelfContainedServiceProviderActivator(provider))

                // Connection with Stream Deck.
                .AddSingleton(registrationParameters)
                .AddSingleton<IStreamDeckConnection>(connection)
                .AddSingleton<IStreamDeckConnectionController>(connection)

                // Action interactivity.
                .AddSingleton<IDrillDownFactory, DrillDownFactory>()
                .AddSingleton<IStreamDeckActionRegistry, StreamDeckActionRegistry>(provider =>
                {
                    var registry = ActivatorUtilities.CreateInstance<StreamDeckActionRegistry>(provider);
                    foreach (var assembly in pluginBuilder.Assemblies)
                    {
                        registry.RegisterAll(assembly);
                    }

                    return registry;
                })

                // Host
                .AddSingleton<IHostedService, StreamDeckPluginService>();
        }
    }
}
