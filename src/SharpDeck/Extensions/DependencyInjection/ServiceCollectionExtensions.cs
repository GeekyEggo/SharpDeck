namespace SharpDeck.Extensions.DependencyInjection
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SharpDeck.Connectivity;
    using SharpDeck.Connectivity.Net;
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
        public static IServiceCollection AddStreamDeck(this IServiceCollection services, Action<PluginContext> configurePlugin = default)
        {
            return services
                // Connection with Stream Deck.
                .AddSingleton(new RegistrationParameters(Environment.GetCommandLineArgs()))
                .AddSingleton<IStreamDeckConnection>(provider => provider.GetRequiredService<StreamDeckWebSocketConnection>())
                .AddSingleton<StreamDeckWebSocketConnection>()

                // Action interactivity.
                .AddSingleton<IDynamicProfileFactory, DynamicProfileFactory>()

                // Hosts
                .AddSingleton<IHostedService>(provider =>
                {
                    // Construct the default action registry.
                    var actionRegistry = ActivatorUtilities.CreateInstance<StreamDeckActionRegistry>(provider);
                    actionRegistry.RegisterAll(Assembly.GetEntryAssembly());

                    // Apply the configuration to the context.
                    var context = new PluginContext(actionRegistry, provider.GetRequiredService<IStreamDeckConnection>(), provider.GetRequiredService<RegistrationParameters>());
                    configurePlugin?.Invoke(context);

                    return actionRegistry;
                })
                .AddSingleton<IHostedService>(provider => provider.GetRequiredService<StreamDeckWebSocketConnection>());
        }
    }
}
