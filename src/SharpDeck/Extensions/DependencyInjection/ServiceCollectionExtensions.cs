namespace SharpDeck.Extensions.DependencyInjection
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SharpDeck.Connectivity;
    using SharpDeck.Connectivity.Net;
    using SharpDeck.Events.Received;
    using SharpDeck.Interactivity;

    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures and adds a new Stream Deck plugin as an <see cref="IHostedService"/>.
        /// </summary>
        /// <param name="services">This instance.</param>
        /// <returns>The service collection for chaining.</returns>
        internal static IServiceCollection AddStreamDeck(this IServiceCollection services)
        {
            return services
                // Connection with Stream Deck.
                .AddSingleton(new RegistrationParameters(Environment.GetCommandLineArgs()))
                .AddSingleton<IStreamDeckConnection>(provider => provider.GetRequiredService<StreamDeckWebSocketConnection>())
                .AddSingleton<StreamDeckWebSocketConnection>()

                // Actions and interactivity.
                .AddSingleton<IDynamicProfileFactory, DynamicProfileFactory>()
                .AddSingleton<IStreamDeckActionRegistry>(provider =>
                {
                    // Construct the default action registry.
                    var actionRegistry = ActivatorUtilities.CreateInstance<StreamDeckActionRegistry>(provider);
                    actionRegistry.RegisterAll(Assembly.GetEntryAssembly());

                    return actionRegistry;
                });
        }
    }
}
