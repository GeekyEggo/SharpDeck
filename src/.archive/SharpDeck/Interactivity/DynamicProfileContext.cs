namespace SharpDeck.Interactivity
{
    using System;
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides contextual information about the current dynamic profile.
    /// </summary>
    /// <typeparam name="T">The type of the items within the dynamic profile.</typeparam>
    public class DynamicProfileContext<T>
    {
        /// <summary>
        /// Occurs when a close is requested.
        /// </summary>
        internal event EventHandler<DynamicProfileResult<T>> CloseRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProfileContext{TItem}" /> struct.
        /// </summary>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="pluginUUID">The unique identifier of the plugin.</param>
        /// <param name="device">The device information.</param>
        /// <param name="profileName">The name of the profile.</param>
        internal DynamicProfileContext(IStreamDeckConnection connection, string pluginUUID, IDevice device, string profileName)
        {
            this.Connection = connection;
            this.Device = device;
            this.PluginUUID = pluginUUID;
            this.ProfileName = profileName;
        }

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        public IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the device information.
        /// </summary>
        public IDevice Device { get; }

        /// <summary>
        /// Gets the dynamic profile.
        /// </summary>
        public IDynamicProfile<T> Profile { get; internal set; }

        /// <summary>
        /// Gets the unique identifier of the plugin.
        /// </summary>
        public string PluginUUID { get; }

        /// <summary>
        /// Gets the name of the profile.
        /// </summary>
        public string ProfileName { get; }
    }
}
