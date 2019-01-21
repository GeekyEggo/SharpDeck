namespace SharpDeck.Messages
{
    /// <summary>
    /// Provides information about a device-based message being sent to an Elgato Stream Deck.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    public class DeviceMessage<TPayload> : ContextMessage<TPayload>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceMessage{TPayload}" /> class.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="context">An opaque value identifying the plugin. This value should be set to the PluginUUID received during the registration procedure.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="payload">The payload.</param>
        public DeviceMessage(string @event, string context, string device, TPayload payload)
            : base(@event, context, payload)
        {
            this.Device = device;
        }

        /// <summary>
        /// Gets or sets the opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Device { get; set; }
    }
}
