namespace SharpDeck.Exceptions
{
    using System;

    /// <summary>
    /// An exception thrown when a registered Stream Deck action throws an unhandled exception.
    /// </summary>
    public class ActionInvokeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvokeException"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="innerException">The inner exception.</param>
        public ActionInvokeException(string context, Exception innerException)
            : base(innerException.Message, innerException)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets the context of the action.
        /// </summary>
        public string Context { get; }
    }
}
