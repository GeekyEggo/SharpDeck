namespace SharpDeck.PropertyInspectors
{
    using System;
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides a decorator for methods, allowing them to be automatically triggered by <see cref="IStreamDeckConnection.SendToPlugin" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PropertyInspectorMethodAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorMethodAttribute"/> class.
        /// </summary>
        public PropertyInspectorMethodAttribute()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorMethodAttribute"/> class.
        /// </summary>
        /// <param name="sendToPluginEvent">The `sendToPlugin` event name; this will also be used as the returning event when <see cref="SendToPropertyInspectorEvent"/> is null, but there is a return type.</param>
        public PropertyInspectorMethodAttribute(string sendToPluginEvent)
            : this(sendToPluginEvent, sendToPluginEvent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorMethodAttribute" /> class.
        /// </summary>
        /// <param name="sendToPluginEvent">The `sendToPlugin` event name; this will be used to intercept events received from the property inspector.</param>
        /// <param name="sendToPropertyInspectorEvent">The `sendToPropertyInspectorEvent` event name; this will be used when sending the event to the property inspector.</param>
        public PropertyInspectorMethodAttribute(string sendToPluginEvent, string sendToPropertyInspectorEvent)
        {
            this.SendToPluginEvent = sendToPluginEvent;
            this.SendToPropertyInspectorEvent = sendToPropertyInspectorEvent;
        }

        /// <summary>
        /// Gets the `sendToPlugin` event name.
        /// </summary>
        public string SendToPluginEvent { get; }

        /// <summary>
        /// Gets the `sendToPropertyInspector` event name.
        /// </summary>
        public string SendToPropertyInspectorEvent { get; }
    }
}
