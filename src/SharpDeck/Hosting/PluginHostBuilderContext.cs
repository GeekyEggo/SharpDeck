namespace SharpDeck.Hosting
{
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides contextual information about the plugin host builder.
    /// </summary>
    public struct PluginHostBuilderContext
    {
        /// <summary>
        /// Gets the connection that will be used to communicate with the Stream Deck.
        /// </summary>
        public IStreamDeckConnection Connection { get; internal set; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        public RegistrationParameters RegistrationParameters { get; internal set; }
    }
}
