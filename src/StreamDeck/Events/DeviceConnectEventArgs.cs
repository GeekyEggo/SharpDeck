namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides information about a device connection event received from an Elgato Stream Deck.
    /// </summary>
    public class DeviceConnectEventArgs : DeviceEventArgs
    {
        /// <summary>
        /// Gets the information about the device.
        /// </summary>
        [JsonInclude]
        public DeviceInfo? DeviceInfo { get; internal set; }
    }
}
