namespace SharpDeck
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using SharpDeck.Connectivity;
    using SharpDeck.Connectivity.Net;
    using SharpDeck.DependencyInjection;
    using SharpDeck.Events.Received;
    using SharpDeck.Hosting;
    using SharpDeck.Interactivity;

    /// <summary>
    /// Provides a builder that allows for configuration of a <see cref="IPluginHost"/>.
    /// </summary>
    public class PluginHostBuilder : IPluginHostBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginHostBuilder"/> class.
        /// </summary>
        public PluginHostBuilder()
        {
            this.RegistrationParameters = new RegistrationParameters(Environment.GetCommandLineArgs());
            this.Connection = new StreamDeckWebSocketConnection(this.RegistrationParameters);
        }

        /// <summary>
        /// Gets the collection of delegates responsible for configuring the underlying <see cref="IStreamDeckActionRegistry"/>.
        /// </summary>
        private IList<Action<IStreamDeckActionRegistry>> ConfigureActionsDelegates { get; } = new List<Action<IStreamDeckActionRegistry>>();

        /// <summary>
        /// Gets the collection of delegates responsible for configuring the <see cref="Services"/>.
        /// </summary>
        private IList<Action<PluginHostBuilderContext, IServiceCollection>> ConfigureServicesDelegates { get; } = new List<Action<PluginHostBuilderContext, IServiceCollection>>();

        /// <summary>
        /// Gets the connection that will be used to communicate with the Stream Deck.
        /// </summary>
        private StreamDeckWebSocketConnection Connection { get; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        private IServiceProvider Services { get; set; }

        /// <summary>
        /// Builds the host from this instances configuration.
        /// </summary>
        /// <returns>The plugin host.</returns>
        public IPluginHost Build()
        {
            this.CreateServiceProvider();
            this.RegisterActions();

            return this.Services.GetRequiredService<IPluginHost>();
        }

        /// <summary>
        /// Configures the actions within the <see cref="IPluginHost"/>.
        /// </summary>
        /// <param name="configure">The delegate responsible for configuring the actions.</param>
        /// <returns>This instance.</returns>
        public IPluginHostBuilder ConfigureActions(Action<IStreamDeckActionRegistry> configure)
        {
            this.ConfigureActionsDelegates.Add(configure);
            return this;
        }

        /// <summary>
        /// Configures the service provider to be used by the <see cref="IPluginHost"/>.
        /// </summary>
        /// <param name="configure">The delegate responsible for configuring the service provider.</param>
        /// <returns>This instance.</returns>
        public IPluginHostBuilder ConfigureServices(Action<PluginHostBuilderContext, IServiceCollection> configure)
        {
            this.ConfigureServicesDelegates.Add(configure);
            return this;
        }

        /// <summary>
        /// Creates the service provider.
        /// </summary>
        private void CreateServiceProvider()
        {
            var services = new ServiceCollection()
                // Misc
                .AddSingleton<IActivator, SelfContainedServiceProviderActivator>(serviceProvider => new SelfContainedServiceProviderActivator(serviceProvider))

                // Connection with Stream Deck.
                .AddSingleton(this.RegistrationParameters)
                .AddSingleton<IStreamDeckConnection>(this.Connection)
                .AddSingleton<IStreamDeckConnectionController>(this.Connection)

                // Action interactivity.
                .AddSingleton<IDrillDownFactory, DrillDownFactory>()
                .AddSingleton<IStreamDeckActionRegistry, StreamDeckActionRegistry>()

                // Host
                .AddSingleton<IPluginHost, PluginHost>();

            var hostBuilderContext = new PluginHostBuilderContext
            {
                Connection = this.Connection,
                RegistrationParameters = this.RegistrationParameters
            };

            foreach (var configure in this.ConfigureServicesDelegates)
            {
                configure(hostBuilderContext, services);
            }

            this.Services = services.BuildServiceProvider();
        }

        /// <summary>
        /// Registers the Stream Deck actions.
        /// </summary>
        private void RegisterActions()
        {
            var registry = this.Services.GetRequiredService<IStreamDeckActionRegistry>();
            registry.RegisterAll(Assembly.GetEntryAssembly());

            foreach (var configure in this.ConfigureActionsDelegates)
            {
                configure(registry);
            }
        }
    }
}
