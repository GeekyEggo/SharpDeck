namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about a device-based event received from an Elgato Stream Deck.
    /// </summary>
    public class DeviceEventArgs : StreamDeckEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceEventArgs"/> class.
        /// </summary>
        /// <param name="event">The name of the event.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        public DeviceEventArgs(string @event, string device)
            : base(@event) => this.Device = device;

        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Device { get; }
    }
}
