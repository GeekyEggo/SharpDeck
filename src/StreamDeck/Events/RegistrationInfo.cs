namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about registration; this is used when initialising a connection with the Stream.
    /// </summary>
    public class RegistrationInfo
    {
        /// <summary>
        /// Gets an object containing information about the application.
        /// </summary>
        [JsonInclude]
        public Application? Application { get; internal set; }

        /// <summary>
        /// Gets the device pixel ratio.
        /// </summary>
        [JsonInclude]
        public int? DevicePixelRatio { get; internal set; }

        /// <summary>
        /// Gets the plugin information.
        /// </summary>
        [JsonInclude]
        public PluginInfo? Plugin { get; internal set; }

        /// <summary>
        /// Gets an array of devices.
        /// </summary>
        [JsonInclude]
        public IdentifiableDeviceInfo[]? Devices { get; internal set; }
    }
}
