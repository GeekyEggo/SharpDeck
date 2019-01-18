namespace SharpDeck.Events
{
    using System;

    /// <summary>
    /// An attribute that acts as an identifier, and indicates that an event represents a message supplied by an Elgato Stream Deck.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event)]
    public class StreamDeckEventAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckEventAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public StreamDeckEventAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }
    }
}
