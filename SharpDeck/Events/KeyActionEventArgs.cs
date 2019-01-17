namespace SharpDeck.Events
{
    using Models;

    /// <summary>
    /// Provides information about an key-based event received from an Elgato Stream Deck.
    /// </summary>
    public class KeyActionEventArgs : ActionEventArgs<KeyPayload>
    {
    }
}
