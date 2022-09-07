namespace StreamDeck
{
    using System;

    /// <summary>
    /// Provides information about a profile supplied with the plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class ProfileAttribute : SerializableSafeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileAttribute"/> class.
        /// </summary>
        /// <param name="name">The filename of the profile.</param>
        /// <param name="deviceType">The type of the device.</param>
        public ProfileAttribute(string name, Device deviceType)
            : base()
        {
            this.Name = name;
            this.DeviceType = deviceType;
        }

        /// <summary>
        /// Gets the filename of the profile.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the type of the device.
        /// </summary>
        public Device DeviceType { get; internal set; }

        /// <summary>
        /// Gets or sets the value indicating whether to mark the profile as read-only; <c>false</c> by default.
        /// </summary>
        public bool Readonly { get; set; } = false;

        /// <summary>
        /// Gets or sets the value indicating whether to prevent Stream Deck from automatically switching to this profile when installed; <c>false</c> by default.
        /// </summary>
        public bool DontAutoSwitchWhenInstalled { get; set; } = false;
    }
}
