namespace SharpDeck.Events.Received
{
    using System;

    /// <summary>
    /// Provides an attribute that can be used to decorate a method that is associated with an event from a Stream Deck.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class StreamDeckEventAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckEventAttribute"/> class.
        /// </summary>
        /// <param name="event">The event name.</param>
        public StreamDeckEventAttribute(string @event)
        {
            this.Event = @event;
        }

        /// <summary>
        /// Gets the event name.
        /// </summary>
        public string Event { get; }
    }
}
