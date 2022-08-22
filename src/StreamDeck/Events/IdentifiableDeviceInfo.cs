namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about a device, that is identifiable.
    /// </summary>
    public class IdentifiableDeviceInfo : DeviceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableDeviceInfo"/> class.
        /// </summary>
        /// <param name="id">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="name">The device name.</param>
        /// <param name="size">The number of columns and rows of keys that the device owns.</param>
        /// <param name="type">The type of device.</param>
        public IdentifiableDeviceInfo(string id, string name, Size size, Device type)
            : base(name, size, type) => this.Id = id;

        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Id { get; }
    }
}
