namespace StreamDeck.Extensions.Hosting
{
    using System.Runtime.Versioning;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using StreamDeck.Routing;

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

#if NET6_0_OR_GREATER
        /// <summary>
        /// Maps the <see cref="IStreamDeckActionIdentifier.UUID"/> to the <typeparamref name="TAction"/> type, allowing for <see cref="IStreamDeckConnection"/> events to be routed to an action instance.
        /// </summary>
        /// <typeparam name="TAction">The type of the action to route events to.</typeparam>
        /// <param name="host">The <see cref="IHost"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        [RequiresPreviewFeatures]
        public static IHost MapAction<TAction>(this IHost host)
            where TAction : StreamDeckAction, IStreamDeckActionIdentifier
            => host.MapAction<TAction>(TAction.UUID);
#endif

        /// <summary>
        /// Maps the specified action <paramref name="uuid"/> to the <typeparamref name="TAction"/> type, allowing for <see cref="IStreamDeckConnection"/> events to be routed to an action instance.
        /// </summary>
        /// <typeparam name="TAction">The type of the action to route events to.</typeparam>
        /// <param name="host">The <see cref="IHost"/>.</param>
        /// <param name="uuid">The the unique identifier of the action; see <see href="https://developer.elgato.com/documentation/stream-deck/sdk/manifest/#actions"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapAction<TAction>(this IHost host, string uuid)
            where TAction : StreamDeckAction
        {
            host.Services
                .GetRequiredService<ActionRouter>()
                .MapAction<TAction>(uuid);

            return host;
        }
    }
}
