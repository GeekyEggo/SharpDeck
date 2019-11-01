
namespace SharpDeck.Exceptions
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides event arguments used when an exception is encountered during a Stream Deck connection.
    /// </summary>
    public class StreamDeckConnectionErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckConnectionErrorEventArgs" /> class.
        /// </summary>
        /// <param name="event">The event name.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="message">The original message.</param>
        /// <param name="exception">The exception.</param>
        public StreamDeckConnectionErrorEventArgs(string @event, JObject args, string message, Exception exception = null)
        {
            this.Args = args;
            this.Event = @event;
            this.Exception = exception;
            this.Message = message;
        }

        /// <summary>
        /// Gets the arguments as a <see cref="JObject"/>, parsed from the <see cref="Message"/>.
        /// </summary>
        public JObject Args { get; }

        /// <summary>
        /// Gets the event parsed from the <see cref="Message"/>.
        /// </summary>
        public string Event { get; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the original message.
        /// </summary>
        public string Message { get; }
    }
}
