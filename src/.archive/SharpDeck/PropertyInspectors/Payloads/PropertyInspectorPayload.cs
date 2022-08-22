namespace SharpDeck.PropertyInspectors.Payloads
{
    /// <summary>
    /// Provides a base class for a payload used when requesting / responding to a property inspector method invocation.
    /// </summary>
    internal class PropertyInspectorPayload
    {
        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public string RequestId { get; set; }
    }
}
