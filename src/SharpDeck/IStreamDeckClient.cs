namespace SharpDeck
{
    using SharpDeck.Events;
    using SharpDeck.Exceptions;

    /// <summary>
    /// Provides methods, events and properties for controlling and communicating with an Elgato Stream Deck.
    /// </summary>
    public interface IStreamDeckClient : IStreamDeckSender, IStreamDeckActionEventPropogator, IStreamDeckEventPropagator
    {
        /// <summary>
        /// Occurs when the client encounters an error.
        /// </summary>
        event StreamDeckClientEventHandler<StreamDeckConnectionErrorEventArgs> Error;
    }
}
