namespace SharpDeck.Hosting
{
    using System;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides contextual information about the plugin host builder.
    /// </summary>
    public struct PluginHostBuilderContext
    {
        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        public RegistrationParameters RegistrationParameters { get; internal set; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        public IServiceProvider Services { get; internal set; }
    }
}
