namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about a device.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Gets the device name.
        /// </summary>
        public string? Name { get; internal set; }

        /// <summary>
        /// Gets the number of columns and rows of keys that the device owns.
        /// </summary>
        public Size? Size { get; internal set; }

        /// <summary>
        /// Gets the type of device.
        /// </summary>
        public DeviceType? Type { get; internal set; }
    }
}
