namespace SharpDeck.PropertyInspectors.Payloads
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides the response payload for a method invoked from the property inspector.
    /// </summary>
    internal class PropertyInspectorResponsePayload : PropertyInspectorRequestPayload
    {
        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        [JsonProperty("data")]
        public object Content { get; set; }
    }
}
