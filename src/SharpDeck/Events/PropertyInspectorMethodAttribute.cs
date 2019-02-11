namespace SharpDeck.Events
{
    using System;

    /// <summary>
    /// Provides a decorator for methods, allowing them to be automatically triggered by <see cref="StreamDeckActionReceiver.SendToPlugin"/>.
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
        {
            this.SendToPluginEvent = sendToPluginEvent;
            this.SendToPropertyInspectorEvent = sendToPluginEvent;
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
