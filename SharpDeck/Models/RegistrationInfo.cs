namespace SharpDeck.Models
{
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
