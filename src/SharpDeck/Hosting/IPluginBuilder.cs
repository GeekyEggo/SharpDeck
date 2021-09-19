namespace SharpDeck.Hosting
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides information and methods for building a Stream Deck plugin.
    /// </summary>
    public interface IPluginBuilder
    {
        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        RegistrationParameters RegistrationParameters { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Adds the specified assembly to the plugin; all instances of <see cref="StreamDeckAction"/> or <see cref="StreamDeckAction{TSettings}"/> that have the attribute <see cref="StreamDeckActionAttribute"/> will be intercepted.
        /// </summary>
        /// <param name="assembly">The assembly to add.</param>
        /// <returns>The plugin builder for chaining.</returns>
        IPluginBuilder AddAssembly(Assembly assembly);
    }
}
