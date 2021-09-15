namespace SharpDeck.Interactivity
{
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides contextual information about the current drill-down.
    /// </summary>
    public struct DrillDownContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrillDownContext"/> struct.
        /// </summary>
        /// <param name="connection">The connection to the Stream Deck.</param>
        /// <param name="pluginUUID">The unique identifier of the plugin.</param>
        /// <param name="device">The device information.</param>
        internal DrillDownContext(IStreamDeckConnection connection, string pluginUUID, IDevice device)
        {
            this.Connection = connection;
            this.PluginUUID = pluginUUID;
            this.Device = device;
        }

        /// <summary>
        /// Gets the connection to the Stream Deck.
        /// </summary>
        public IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the device information.
        /// </summary>
        public IDevice Device { get; }

        /// <summary>
        /// Gets the unique identifier of the plugin.
        /// </summary>
        public string PluginUUID { get; }
    }
}
