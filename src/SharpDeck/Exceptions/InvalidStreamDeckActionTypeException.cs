namespace SharpDeck.Exceptions
{
    using System;

    /// <summary>
    /// An exception thrown during automatical initialization of the <see cref="StreamDeckPlugin"/>.
    /// </summary>
    public class InvalidStreamDeckActionTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStreamDeckActionTypeException"/> class.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        public InvalidStreamDeckActionTypeException(Type actionType)
        {
            this.ActionType = actionType;
        }

        /// <summary>
        /// Gets the type of the action that was attempted to be registered.
        /// </summary>
        public Type ActionType { get; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
            => $"Unable to register action {this.ActionType.FullName}, it does not inherit from {typeof(StreamDeckAction).FullName}";
    }
}
