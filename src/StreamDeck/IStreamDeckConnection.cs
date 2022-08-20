namespace StreamDeck
{
    using StreamDeck.Events;

    /// <summary>
    /// Provides a connection to a Stream Deck.
    /// </summary>
    public partial interface IStreamDeckConnection : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Gets the information supplied by Stream Deck when establishing the connection.
        /// </summary>
        RegistrationInfo Info { get; }
    }
}
