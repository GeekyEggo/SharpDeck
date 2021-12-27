namespace SharpDeck.Hosting
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SharpDeck.Connectivity.Net;

    /// <summary>
    /// Provides a <see cref="IHost"/> that remains alive as long as there is an active connection to the Stream Deck.
    /// </summary>
    internal sealed class StreamDeckPluginHost : IHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckPluginHost"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        public StreamDeckPluginHost(IServiceProvider services)
        {
            this.Services = services;
            this.HostedServices = services.GetServices<IHostedService>().ToArray();
        }

        /// <inheritdoc/>
        public IServiceProvider Services { get; set; }

        /// <summary>
        /// Gets the services to host.
        /// </summary>
        private IHostedService[] HostedServices { get; }

        /// <inheritdoc/>
        public void Dispose()
            => GC.SuppressFinalize(this);

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            foreach (var hostedService in this.HostedServices)
            {
                await hostedService.StartAsync(cancellationToken);
            }

            await this.Services.GetRequiredService<StreamDeckWebSocketConnection>()
                .WaitForDisconnectAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            foreach (var hostedService in this.HostedServices)
            {
                await hostedService.StopAsync(cancellationToken);
            }
        }
    }
}
