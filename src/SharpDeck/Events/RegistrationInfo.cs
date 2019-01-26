namespace SharpDeck.Events
{
    /// <summary>
    /// Provides information about registration; this is used when initialising a <see cref="StreamDeckClient"/>, and is typically supplied as an argument.
    /// </summary>
    public class RegistrationInfo
    {
        /// <summary>
        /// Gets or sets an object containing information about the application.
        /// </summary>
        public Application Application { get; set; }

        /// <summary>
        /// Gets or sets an array of devices.
        /// </summary>
        public IdentifiableDeviceInfo[] Devices { get; set; }
    }
}
