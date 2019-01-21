namespace SharpDeck
{
    using System;

    /// <summary>
    /// Provides information about an error encountered by <see cref="StreamDeckClient"/>.
    /// </summary>
    public class StreamDeckClientErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckClientErrorEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public StreamDeckClientErrorEventArgs(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets the error message message.
        /// </summary>
        public string Message { get; }
    }
}
