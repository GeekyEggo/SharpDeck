namespace SharpDeck
{
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides a connection to a Stream Deck, with additional functionality for managing the connection.
    /// </summary>
    internal interface IStreamDeckConnectionController : IStreamDeckConnection
    {
        /// <summary>
        /// Initiates a connection to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        Task ConnectAsync(RegistrationParameters registrationParameters, CancellationToken cancellationToken = default);
    }
}
