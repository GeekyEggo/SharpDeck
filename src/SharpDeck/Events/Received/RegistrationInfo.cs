namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Provides information about registration; this is used when initialising a connection with the Stream.
    /// </summary>
    public class RegistrationInfo
    {
        /// <summary>
        /// Gets or sets an object containing information about the application.
        /// </summary>
        public Application Application { get; set; }

        /// <summary>
        /// Gets or sets the device pixel ratio.
        /// </summary>
        public int DevicePixelRatio { get; set; }

        /// <summary>
        /// Gets or sets the plugin information.
        /// </summary>
        public PluginInfo Plugin { get; set; }

        /// <summary>
        /// Gets or sets an array of devices.
        /// </summary>
        public IdentifiableDeviceInfo[] Devices { get; set; }
    }
}
