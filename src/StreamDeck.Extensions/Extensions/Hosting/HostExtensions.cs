namespace StreamDeck.Extensions.Hosting
{
    using System.Reflection;
    using System.Runtime.Versioning;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using StreamDeck.Routing;

    /// <summary>
    /// Provides extension methods for <see cref="IHost"/>.
    /// </summary>
    public static class HostExtensions
    {
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

        /// <summary>
        /// Maps actions discovered in <paramref name="assemblies"/>; actions must inherit from <see cref="StreamDeckAction"/>, and be decorated with <see cref="ActionAttribute"/>.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/>.</param>
        /// <param name="assemblies">The assemblies to discover actions in.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapActions(this IHost host, params Assembly[] assemblies)
        {
            var actionRouter = host.Services
                .GetRequiredService<ActionRouter>();

            var actionTypes = assemblies
                .Concat(new[] { Assembly.GetEntryAssembly() })
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && typeof(StreamDeckAction).IsAssignableFrom(t));

            foreach (var type in actionTypes)
            {
                var actionAttr = type.GetCustomAttribute<ActionAttribute>();
                if (actionAttr != null)
                {
                    actionRouter.MapAction(actionAttr.UUID, type);
                }
            }

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
    }
}
