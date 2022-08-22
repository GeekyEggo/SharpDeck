namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about registration; this is used when initialising a connection with the Stream.
    /// </summary>
    public class RegistrationInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationInfo"/> class.
        /// </summary>
        /// <param name="application">An object containing information about the application</param>
        /// <param name="colors">The preferred user colors.</param>
        /// <param name="devicePixelRatio">The device pixel ratio.</param>
        /// <param name="plugin">The plugin information.</param>
        /// <param name="devices">The available Stream Deck devices.</param>
        public RegistrationInfo(Application application, Colors colors, int devicePixelRatio, PluginInfo plugin, IdentifiableDeviceInfo[] devices)
        {
            this.Application = application;
            this.Colors = colors;
            this.DevicePixelRatio = devicePixelRatio;
            this.Plugin = plugin;
            this.Devices = devices ?? Array.Empty<IdentifiableDeviceInfo>();
        }

        /// <summary>
        /// Gets an object containing information about the application.
        /// </summary>
        public Application Application { get; }

        /// <summary>
        /// Gets the preferred user colors.
        /// </summary>
        public Colors Colors { get; }

        /// <summary>
        /// Gets the device pixel ratio.
        /// </summary>
        public int DevicePixelRatio { get; }

        /// <summary>
        /// Gets the plugin information.
        /// </summary>
        public PluginInfo Plugin { get; }

        /// <summary>
        /// Gets the available Stream Deck devices.
        /// </summary>
        public IdentifiableDeviceInfo[] Devices { get; }
    }
}
