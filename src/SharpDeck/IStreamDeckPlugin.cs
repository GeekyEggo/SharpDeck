namespace SharpDeck
{
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a wrapper for connecting and communication with a Stream Deck.
    /// </summary>
    public interface IStreamDeckPlugin
    {
        /// <summary>
        /// Gets or sets the assembly containing the <see cref="StreamDeckAction"/> to register.
        /// </summary>
        Assembly Assembly { get; set; }

        /// <summary>
        /// Gets the connection with the Stream Deck responsible for sending and receiving events and messages.
        /// </summary>
        IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Runs the plugin.
        /// </summary>
        void Run();

        /// <summary>
        /// Runs the Stream Deck plugin asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of connecting and maintaining the connection with the Stream Deck.</returns>
        Task RunAsync(CancellationToken cancellationToken = default);
    }
}
