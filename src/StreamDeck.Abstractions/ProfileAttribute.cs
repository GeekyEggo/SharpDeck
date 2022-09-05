namespace StreamDeck
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides information about a profile supplied with the plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class ProfileAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileAttribute"/> class.
        /// </summary>
        /// <param name="name">The filename of the profile.</param>
        /// <param name="deviceType">The type of the device.</param>
        public ProfileAttribute(string name, Device deviceType)
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
        public Device DeviceType { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether to mark the profile as read-only; <c>false</c> by default.
        /// </summary>
        public bool? Readonly { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether to prevent Stream Deck from automatically switching to this profile when installed; <c>false</c> by default.
        /// </summary>
        public bool? DontAutoSwitchWhenInstalled { get; set; }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public override object TypeId => base.TypeId;
    }
}
