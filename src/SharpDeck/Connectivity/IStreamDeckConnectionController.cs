namespace SharpDeck.Connectivity
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a connection to a Stream Deck, with additional functionality for managing the connection.
    /// </summary>
    internal interface IStreamDeckConnectionController : IStreamDeckConnection
    {
        /// <summary>
        /// Initiates a connection to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disconnects the connection to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        Task DisconnectAsync(CancellationToken cancellationToken = default);
    }
}
