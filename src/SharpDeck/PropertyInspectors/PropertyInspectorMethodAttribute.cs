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
        /// <param name="eventName">The name of the event used to identify the method within the Stream Deck action; this is returned to the Stream Deck property inspector following method invocation.</param>
        public PropertyInspectorMethodAttribute(string eventName = "")
            => this.EventName = eventName;

        /// <summary>
        /// Gets or sets the name of the event used to identify the method within the Stream Deck action; this is returned to the Stream Deck property inspector following method invocation. Defaults to the decorated method name.
        /// </summary>
        public string EventName { get; set; }
    }
}
