namespace SharpDeck.PropertyInspectors.Payloads
{
    /// <summary>
    /// Provides the response payload for a method invoked from the property inspector.
    /// </summary>
    internal class PropertyInspectorResponsePayload : PropertyInspectorPayload
    {
        /// <summary>
        /// Gets or sets the content of the response.
        /// </summary>
        public object Data { get; set; }
    }
}
