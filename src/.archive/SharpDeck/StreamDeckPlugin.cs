namespace SharpDeck
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using SharpDeck.Extensions.Hosting;

    /// <summary>
    /// Provides static methods for starting the Stream Deck plugin.
    /// </summary>
    public static class StreamDeckPlugin
    {
        /// <summary>
        /// Runs the Stream Deck plugin.
        /// </summary>
        public static void Run()
            => new HostBuilder().RunStreamDeckPlugin();

        /// <summary>
        /// Runs the Stream Deck plugin asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token that can be used to stop the plugin.</param>
        /// <returns>The task of running the Stream Deck plugin.</returns>
        public static Task RunAsync(CancellationToken cancellationToken = default)
            => new HostBuilder().RunStreamDeckPluginAsync(cancellationToken);
    }
}
