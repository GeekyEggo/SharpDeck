namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about a device.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfo"/> class.
        /// </summary>
        /// <param name="name">The device name.</param>
        /// <param name="size">The number of columns and rows of keys that the device owns.</param>
        /// <param name="type">The type of device.</param>
        public DeviceInfo(string name,  Size size, Device type)
        {
            this.Name = name;
            this.Size = size;
            this.Type = type;
        }

        /// <summary>
        /// Gets the device name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the number of columns and rows of keys that the device owns.
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// Gets the type of device.
        /// </summary>
        public Device Type { get; }
    }
}
