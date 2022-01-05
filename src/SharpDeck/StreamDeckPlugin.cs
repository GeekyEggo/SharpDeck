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
        /// Gets the host.
        /// </summary>
        private static IHost Host { get; } = new HostBuilder().UseStreamDeck().Build();

        /// <summary>
        /// Runs the Stream Deck plugin.
        /// </summary>
        public static void Run()
            => Host.Run();

        /// <summary>
        /// Runs the Stream Deck plugin asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of running the Stream Deck plugin.</returns>
        public static Task RunAsync(CancellationToken cancellationToken = default)
            => Host.RunAsync(cancellationToken);
    }
}
