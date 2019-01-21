namespace SharpDeck.Events
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides information about a method that is decorated with <see cref="StreamDeckEventAttribute"/>, identifying it as an event that can be triggered by an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckEventInfo"/> class.
        /// </summary>
        /// <param name="attr">The attribute.</param>
        /// <param name="methodInfo">The method information.</param>
        public StreamDeckEventInfo(StreamDeckEventAttribute attr, MethodInfo methodInfo)
        {
            this.Attribute = attr;
            this.ArgsType = methodInfo.GetParameters()[0].ParameterType;
            this.MethodInfo = methodInfo;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        public StreamDeckEventAttribute Attribute { get; }

        /// <summary>
        /// Gets the type of the arguments that should be supplied when invoking <see cref="MethodInfo"/>.
        /// </summary>
        public Type ArgsType { get; }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        public MethodInfo MethodInfo { get; }
    }
}
