namespace SharpDeck.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods for controlling the plug-in host responsible handling all communications and interactions.
    /// </summary>
    public interface IPluginHost
    {
        /// <summary>
        /// Starts the plugin host.
        /// </summary>
        void Start();

        /// <summary>
        /// Starts the plugin host asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task of running the host.</returns>
        Task StartAsync(CancellationToken cancellationToken = default);
    }
}
