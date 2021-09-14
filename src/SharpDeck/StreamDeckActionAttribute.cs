namespace SharpDeck
{
    using System;

    /// <summary>
    /// Provides identifier information for a Stream Deck action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class StreamDeckActionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionAttribute"/> class.
        /// </summary>
        /// <param name="uuid">The unique identifier of the action.</param>
        public StreamDeckActionAttribute(string uuid)
        {
            this.UUID = uuid;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the action. It must be a uniform type identifier (UTI) that contains only alphanumeric characters (A-Z, a-z, 0-9), hyphen (-), and period (.). The string must be in reverse-DNS format. For example, if your domain is elgato.com and you create a plugin named Hello with the action My Action, you could assign the string com.elgato.hello.myaction as your action's Unique Identifier.
        /// </summary>
        public string UUID { get; set; }
    }
}
