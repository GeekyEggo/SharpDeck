namespace SharpDeck.Models
{
    using Enums;

    /// <summary>
    /// Provides information about a device.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Gets or sets the number of columns and rows of keys that the device owns.
        /// </summary>
        public Coordinates Size { get; set; }

        /// <summary>
        /// Gets or sets the type of device.
        /// </summary>
        public DeviceType Type { get; set; }
    }
}
