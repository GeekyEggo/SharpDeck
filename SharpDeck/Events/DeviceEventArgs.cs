namespace SharpDeck.Events
{
    public class DeviceEventArgs : StreamDeckEventArgs
    {
        /// <summary>
        /// Gets or sets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public object Device { get; set; }
    }
}
