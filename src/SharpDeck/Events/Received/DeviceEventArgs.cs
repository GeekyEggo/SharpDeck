namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Provides information about a device-based event received from an Elgato Stream Deck.
    /// </summary>
    public class DeviceEventArgs : StreamDeckEventArgs
    {
        /// <summary>
        /// Gets or sets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the device information.
        /// </summary>
        public DeviceInfo DeviceInfo { get; set; }
    }
}
