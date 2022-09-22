namespace StreamDeck.Extensions.Hosting
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using StreamDeck.Events;
    using StreamDeck.Routing;

    /// <summary>
    /// Provides extension methods for <see cref="IHost"/>.
    /// </summary>
    public static class HostExtensions
    {
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

        /// <summary>
        /// Applies the delegate to the <see cref="IStreamDeckConnection"/> before connecting to the Stream Deck.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="configure">The delegate for configuring the <see cref="IHost"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapConnection(this IHost host, Action<IStreamDeckConnection> configure)
        {
            configure(host.Services.GetRequiredService<IStreamDeckConnection>());
            return host;
        }

        /// <summary>
        /// Maps the <paramref name="action"/> to the <see cref="IStreamDeckConnection"/>, with parameters resolved from the <see cref="IServiceProvider"/>.
        /// <see cref="IStreamDeckConnection"/> and <see cref="ActionEventArgs{KeyPayload}"/> are propagated directly from the <see cref="IStreamDeckConnection.KeyDown"/> event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.KeyDown"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapKeyDown(this IHost host, Delegate action)
            => host.MapEvent<ActionEventArgs<KeyPayload>>(action, (conn, handler) => conn.KeyDown += handler, args => args.Context);

        /// <summary>
        /// Compiles the <paramref name="action"/>, and invokes <paramref name="addHandler"/> allowing the event handler to be added to the <see cref="IStreamDeckConnection"/>.
        /// </summary>
        /// <typeparam name="TArgs">The type of the event arguments.</typeparam>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action that will be invoked by the <see cref="IStreamDeckConnection"/>.</param>
        /// <param name="addHandler">The delegate responsible for mapping the <paramref name="action"/> to the <see cref="IStreamDeckConnection"/>.</param>
        /// <param name="getContext">The optional factory for selecting the context from the <typeparamref name="TArgs"/>; this is supplied to the <see cref="IDispatcher.Invoke(Func{Task}, string)"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        private static IHost MapEvent<TArgs>(this IHost host, Delegate action, Action<IStreamDeckConnection, EventHandler<IStreamDeckConnection, TArgs>> addHandler, Func<TArgs, string>? getContext = null)
        {
            var handler = action.Compile<TArgs>();
            var dispatcher = host.Services.GetRequiredService<IDispatcher>();

            void eventHandler(IStreamDeckConnection conn, TArgs args) => dispatcher.Invoke(() => handler(host.Services, conn, args), getContext?.Invoke(args) ?? string.Empty);
            addHandler(host.Services.GetRequiredService<IStreamDeckConnection>(), eventHandler);

            return host;
        }
    }
}
