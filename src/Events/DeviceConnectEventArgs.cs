namespace SharpDeck.Events
{
    using Models;

    /// <summary>
    /// Provides information about a device connection event received from an Elgato Stream Deck.
    /// </summary>
    public class DeviceConnectEventArgs : DeviceEventArgs
    {
        /// <summary>
        /// Gets or sets the information about the device.
        /// </summary>
        public DeviceInfo DeviceInfo { get; set; }
    }
}
