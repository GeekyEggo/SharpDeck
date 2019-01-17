namespace SharpDeck.Events
{
    using Models;

    /// <summary>
    /// Provides information about a title-based event received from an Elgato Stream Deck.
    /// </summary>
    public class TitleActionEventArgs : ActionEventArgs<TitlePayload>
    {
    }
}
