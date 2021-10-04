namespace SharpDeck.Hosting
{
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides information about the plugin.
    /// </summary>
    public struct PluginContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginContext"/> class.
        /// </summary>
        /// <param name="actions">The action registry containing all actions registered for the plugin.</param>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="registrationParameters">The registration parameters.</param>
        internal PluginContext(StreamDeckActionRegistry actions, IStreamDeckConnection connection, RegistrationParameters registrationParameters)
        {
            this.Actions = actions;
            this.Connection = connection;
            this.RegistrationParameters = registrationParameters;
        }

        /// <summary>
        /// Gets action registry containing all actions registered for the plugin.
        /// </summary>
        public IStreamDeckActionRegistry Actions { get; }

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        public IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        public RegistrationParameters RegistrationParameters { get; }
    }
}
