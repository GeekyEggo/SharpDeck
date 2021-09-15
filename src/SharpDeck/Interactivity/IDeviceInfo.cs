namespace SharpDeck.Interactivity
{
    using SharpDeck.Enums;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides information about a Stream Deck device.
    /// </summary>
    public interface IDeviceInfo
    {
        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the device name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the number of columns and rows of keys that the device owns.
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Gets the type of device.
        /// </summary>
        DeviceType Type { get; }
    }
}
