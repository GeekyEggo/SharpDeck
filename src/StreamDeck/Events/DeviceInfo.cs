namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides information about a device.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Gets the device name.
        /// </summary>
        [JsonInclude]
        public string? Name { get; internal set; }

        /// <summary>
        /// Gets the number of columns and rows of keys that the device owns.
        /// </summary>
        [JsonInclude]
        public Size? Size { get; internal set; }

        /// <summary>
        /// Gets the type of device.
        /// </summary>
        [JsonInclude]
        public Device? Type { get; internal set; }
    }
}
