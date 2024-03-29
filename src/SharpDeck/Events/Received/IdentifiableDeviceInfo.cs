﻿namespace SharpDeck.Events.Received
{
    using SharpDeck.Interactivity;

    /// <summary>
    /// Provides information about a device, that is identifiable.
    /// </summary>
    public class IdentifiableDeviceInfo : DeviceInfo, IDevice
    {
        /// <summary>
        /// Gets or sets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Id { get; set; }
    }
}
