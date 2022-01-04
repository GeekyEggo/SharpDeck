namespace SharpDeck.PropertyInspectors
{
    /// <summary>
    /// Provides an optional payload which can be used in association with <see cref="PropertyInspectorMethodAttribute"/> to expose a method to the property inspector.
    /// </summary>
    public class PropertyInspectorPayload
    {
        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public object Data { get; set; }
    }
}
