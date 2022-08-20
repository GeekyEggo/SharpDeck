namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about a device, that is identifiable.
    /// </summary>
    public class IdentifiableDeviceInfo : DeviceInfo
    {
        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        [JsonInclude]
        public string? Id { get; internal set; }
    }
}
