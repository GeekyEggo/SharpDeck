namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about a device connection event received from an Elgato Stream Deck.
    /// </summary>
    public class DeviceConnectEventArgs : DeviceEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceConnectEventArgs"/> class.
        /// </summary>
        /// <param name="event">The name of the event.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="deviceInfo">The information about the device.</param>
        public DeviceConnectEventArgs(string @event, string device, DeviceInfo deviceInfo)
            : base(@event, device) => this.DeviceInfo = deviceInfo;

        /// <summary>
        /// Gets the information about the device.
        /// </summary>
        public DeviceInfo DeviceInfo { get; }
    }
}
