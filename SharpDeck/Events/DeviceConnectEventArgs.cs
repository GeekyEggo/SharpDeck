namespace SharpDeck.Events
{
    using Models;
    public class DeviceConnectEventArgs : DeviceEventArgs
    {
        /// <summary>
        /// Gets or sets the information about the device.
        /// </summary>
        public DeviceInfo Info { get; set; }
    }
}
