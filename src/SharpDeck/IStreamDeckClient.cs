namespace SharpDeck
{
    using SharpDeck.Events;

    /// <summary>
    /// Provides methods, events and properties for controlling and communicating with an Elgato Stream Deck.
    /// </summary>
    public interface IStreamDeckClient : IStreamDeckSender, IStreamDeckActionEventPropogator, IStreamDeckEventPropagator
    {
    }
}
