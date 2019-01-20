namespace SharpDeck.Events
{
    using System;

    /// <summary>
    /// An attribute that acts as an identifier, and indicates that an event represents a message supplied by an Elgato Stream Deck.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method, AllowMultiple = false)]
    public class StreamDeckEventAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckEventAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isAction">Determines whether the event is associated with the instance of an action.</param>
        public StreamDeckEventAttribute(string name, bool isAction = false)
        {
            this.IsAction = isAction;
            this.Name = name;
        }

        /// <summary>
        /// Gets a value indicating whether the event is associated with an instance of an action.
        /// </summary>
        public bool IsAction { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }
    }
}
