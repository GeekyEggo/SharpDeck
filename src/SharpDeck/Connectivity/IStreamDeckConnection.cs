namespace SharpDeck.Connectivity
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Events;
    using SharpDeck.Events.Received;
    using SharpDeck.Exceptions;

    /// <summary>
    /// Provides a connection to a Stream Deck.
    /// </summary>
    public interface IStreamDeckConnection : IStreamDeckEventPropagator, IDisposable
    {
        /// <summary>
        /// Occurs when a non-fatal exception is thrown, or an error is encountered.
        /// </summary>
        event EventHandler<StreamDeckConnectionErrorEventArgs> Error;

        /// <summary>
        /// Initiates a connection to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task ConnectAsync(RegistrationParameters registrationParameters, CancellationToken cancellationToken);

        /// <summary>
        /// Disconnects the connection asynchronously.
        /// </summary>
        /// <returns>The task of disconnecting</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Sends the value to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The task of sending the value.</returns>
        Task SendAsync(object value);
    }
}
