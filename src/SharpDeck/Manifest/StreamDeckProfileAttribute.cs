namespace SharpDeck.Manifest
{
    using System;
    using SharpDeck.Enums;

    /// <summary>
    /// Provides information about the location of a Stream Deck profile.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class StreamDeckProfileAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckProfileAttribute"/> class.
        /// </summary>
        /// <param name="name">The filename of the profile.</param>
        /// <param name="deviceType">Type of the device.</param>
        public StreamDeckProfileAttribute(string name, DeviceType deviceType)
        {
            this.Name = name;
            this.DeviceType = deviceType;
        }

        /// <summary>
        /// Gets or sets the filename of the profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the device.
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the profile is read-only.
        /// </summary>
        public bool? ReadOnly { get; set; } = null;
    }
}
