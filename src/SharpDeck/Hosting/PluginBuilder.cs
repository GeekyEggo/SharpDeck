namespace SharpDeck.Hosting
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides information and methods for building a Stream Deck plugin.
    /// </summary>
    internal class PluginBuilder : IPluginBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginBuilder"/> class.
        /// </summary>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="services">The services.</param>
        internal PluginBuilder(IStreamDeckConnection connection, RegistrationParameters registrationParameters, IServiceCollection services)
        {
            this.Assemblies.Add(Assembly.GetEntryAssembly());
            this.Connection = connection;
            this.RegistrationParameters = registrationParameters;
            this.Services = services;
        }

        /// <summary>
        /// Gets the assemblies to be registerd with the plugin.
        /// </summary>
        public List<Assembly> Assemblies { get; } = new List<Assembly>();

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        public IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        public RegistrationParameters RegistrationParameters { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Adds the specified assembly to the plugin; all instances of <see cref="StreamDeckAction" /> or <see cref="StreamDeckAction{TSettings}" /> that have the attribute <see cref="StreamDeckActionAttribute" /> will be intercepted.
        /// </summary>
        /// <param name="assembly">The assembly to add.</param>
        /// <returns>The plugin builder for chaining.</returns>
        public IPluginBuilder AddAssembly(Assembly assembly)
        {
            this.Assemblies.Add(assembly);
            return this;
        }
    }
}
