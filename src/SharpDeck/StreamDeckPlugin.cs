namespace SharpDeck
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SharpDeck.Extensions.DependencyInjection;

    /// <summary>
    /// Provides static methods for starting the Stream Deck plugin.
    /// </summary>
    public static class StreamDeckPlugin
    {
        /// <summary>
        /// Runs the Stream Deck plugin.
        /// </summary>
        public static void Run()
            => Task.WaitAll(RunAsync());

        /// <summary>
        /// Runs the Stream Deck plugin asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of running the Stream Deck plugin.</returns>
        public static Task RunAsync(CancellationToken cancellationToken = default)
            => Task.WhenAll(
                new ServiceCollection()
                .AddStreamDeck()
                .BuildServiceProvider()
                .GetServices<IHostedService>()
                .Select(hostedService => hostedService.StartAsync(cancellationToken))
                .ToArray());
    }
}
