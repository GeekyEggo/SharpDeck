namespace StreamDeck
{
    /// <summary>
    /// Provides methods for connecting to the Stream Deck application.
    /// </summary>
    public interface IStreamDeckConnector : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Connects to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token used to cancel the connection.</param>
        /// <returns>The task of establishing the connection.</returns>
        Task ConnectAsync(CancellationToken cancellationToken = default);
    }
}
