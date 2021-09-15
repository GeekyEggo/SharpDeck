namespace SharpDeck.Interactivity
{
    using System;
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides contextual information about the current drill down.
    /// </summary>
    /// <typeparam name="T">The type of the items within the drill down.</typeparam>
    public class DrillDownContext<T>
    {
        /// <summary>
        /// Occurs when a close is requested.
        /// </summary>
        internal event EventHandler<DrillDownResult<T>> CloseRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrillDownContext{TItem}"/> struct.
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
        /// Gets the drill down.
        /// </summary>
        public IDrillDown<T> DrillDown { get; internal set; }

        /// <summary>
        /// Gets the unique identifier of the plugin.
        /// </summary>
        public string PluginUUID { get; }
    }
}
