namespace SharpDeck.PropertyInspectors.Payloads
{
    /// <summary>
    /// Provides the request payload for a method invoked from the property inspector.
    /// </summary>
    internal class PropertyInspectorRequestPayload
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
