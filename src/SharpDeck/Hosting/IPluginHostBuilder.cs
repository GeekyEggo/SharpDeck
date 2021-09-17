namespace SharpDeck.Hosting
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides a builder that allows for configuration of a <see cref="IPluginHost"/>.
    /// </summary>
    public interface IPluginHostBuilder
    {
        /// <summary>
        /// Builds the host from this instances configuration.
        /// </summary>
        /// <returns>The plugin host.</returns>
        IPluginHost Build();

        /// <summary>
        /// Configures the actions within the <see cref="IPluginHost"/>.
        /// </summary>
        /// <param name="configure">The delegate responsible for configuring the actions.</param>
        /// <returns>This instance.</returns>
        IPluginHostBuilder ConfigureActions(Action<PluginHostBuilderContext, IStreamDeckActionRegistry> configure);

        /// <summary>
        /// Configures the connection to the Stream Deck within the <see cref="IPluginHost"/>.
        /// </summary>
        /// <param name="configure">The delegate responsible for configuring the connection.</param>
        /// <returns>This instance.</returns>
        IPluginHostBuilder ConfigureConnection(Action<PluginHostBuilderContext, IStreamDeckConnection> configure);

        /// <summary>
        /// Configures the service provider to be used by the <see cref="IPluginHost"/>.
        /// </summary>
        /// <param name="configure">The delegate responsible for configuring the service provider.</param>
        /// <returns>This instance.</returns>
        IPluginHostBuilder ConfigureServices(Action<IServiceCollection> configure);
    }
}
