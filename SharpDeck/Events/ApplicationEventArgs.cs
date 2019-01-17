namespace SharpDeck.Events
{
    using Models;

    /// <summary>
    /// Provides information about an application-based event received from an Elgato Stream Deck.
    /// </summary>
    public class ApplicationEventArgs : StreamDeckEventArgs<ApplicationPayload>
    {
    }
}
