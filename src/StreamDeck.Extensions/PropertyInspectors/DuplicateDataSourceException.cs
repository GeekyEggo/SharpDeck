namespace StreamDeck.Extensions.PropertyInspectors
{
    using System;

    /// <summary>
    /// An exception thrown when attempting to register a data source with an event that has already been registerd.
    /// </summary>
    public class DuplicateDataSourceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDataSourceException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DuplicateDataSourceException(string message)
            : base(message) { }
    }
}
