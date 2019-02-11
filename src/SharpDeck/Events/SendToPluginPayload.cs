namespace SharpDeck.Events
{
    /// <summary>
    /// Provides an optional payload which can be used in association with <see cref="PropertyInspectorMethodAttribute"/> to expose a method to the property inspector.
    /// </summary>
    public class SendToPluginPayload
    {
        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        public string Event { get; set; }
    }
}
