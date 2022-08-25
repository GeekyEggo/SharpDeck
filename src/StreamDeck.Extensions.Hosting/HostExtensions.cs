namespace StreamDeck.Extensions.Hosting
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Provides extension methods for <see cref="IHost"/>.
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Applies the delegate <paramref name="configure"/> to the underlying <see cref="IStreamDeckConnection"/> used by the plugin.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="configure">The delegate for configuring the <see cref="IHost"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost ConfigureConnection(this IHost host, Action<IStreamDeckConnection> configure)
        {
            configure(host.Services.GetRequiredService<IStreamDeckConnection>());
            return host;
        }
    }
}
