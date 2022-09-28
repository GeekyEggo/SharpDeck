namespace StreamDeck.Extensions.Hosting
{
    using System;
    using System.Text.Json.Nodes;
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
        /// Maps the delegate to the <see cref="IStreamDeckConnection.ApplicationDidLaunch"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="StreamDeckEventArgs{ApplicationPayload}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.ApplicationDidLaunch"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapApplicationDidLaunch(this IHost host, Delegate action)
            => host.MapEvent<StreamDeckEventArgs<ApplicationPayload>>(action, (conn, handler) => conn.ApplicationDidLaunch += handler);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.ApplicationDidTerminate"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="StreamDeckEventArgs{ApplicationPayload}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.ApplicationDidTerminate"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapApplicationDidTerminate(this IHost host, Delegate action)
            => host.MapEvent<StreamDeckEventArgs<ApplicationPayload>>(action, (conn, handler) => conn.ApplicationDidTerminate += handler);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.DeviceDidConnect"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="DeviceConnectEventArgs"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.DeviceDidConnect"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapDeviceDidConnect(this IHost host, Delegate action)
            => host.MapEvent<DeviceConnectEventArgs>(action, (conn, handler) => conn.DeviceDidConnect += handler);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.DeviceDidDisconnect"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="DeviceEventArgs"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.DeviceDidDisconnect"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapDeviceDidDisconnect(this IHost host, Delegate action)
            => host.MapEvent<DeviceEventArgs>(action, (conn, handler) => conn.DeviceDidDisconnect += handler);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.DidReceiveGlobalSettings"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="StreamDeckEventArgs{SettingsPayload}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.DidReceiveGlobalSettings"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapDidReceiveGlobalSettings(this IHost host, Delegate action)
            => host.MapEvent<StreamDeckEventArgs<SettingsPayload>>(action, (conn, handler) => conn.DidReceiveGlobalSettings += handler);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.DidReceiveSettings"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="ActionEventArgs{ActionPayload}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.DidReceiveSettings"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapDidReceiveSettings(this IHost host, Delegate action)
            => host.MapEvent<ActionEventArgs<ActionPayload>>(action, (conn, handler) => conn.DidReceiveSettings += handler, args => args.Context);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.KeyDown"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="ActionEventArgs{KeyPayload}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.KeyDown"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapKeyDown(this IHost host, Delegate action)
            => host.MapEvent<ActionEventArgs<KeyPayload>>(action, (conn, handler) => conn.KeyDown += handler, args => args.Context);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.KeyUp"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="ActionEventArgs{KeyPayload}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.KeyUp"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapKeyUp(this IHost host, Delegate action)
            => host.MapEvent<ActionEventArgs<KeyPayload>>(action, (conn, handler) => conn.KeyUp += handler, args => args.Context);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.PropertyInspectorDidAppear"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="ActionEventArgs"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.PropertyInspectorDidAppear"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapPropertyInspectorDidAppear(this IHost host, Delegate action)
            => host.MapEvent<ActionEventArgs>(action, (conn, handler) => conn.PropertyInspectorDidAppear += handler, args => args.Context);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.PropertyInspectorDidDisappear"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="ActionEventArgs"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.PropertyInspectorDidDisappear"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapPropertyInspectorDidDisappear(this IHost host, Delegate action)
            => host.MapEvent<ActionEventArgs>(action, (conn, handler) => conn.PropertyInspectorDidDisappear += handler, args => args.Context);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.SendToPlugin"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="PartialActionEventArgs{JsonObject}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.SendToPlugin"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapSendToPlugin(this IHost host, Delegate action)
            => host.MapEvent<PartialActionEventArgs<JsonObject>>(action, (conn, handler) => conn.SendToPlugin += handler, args => args.Context);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.SystemDidWakeUp"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="StreamDeckEventArgs"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.SystemDidWakeUp"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapSystemDidWakeUp(this IHost host, Delegate action)
            => host.MapEvent<StreamDeckEventArgs>(action, (conn, handler) => conn.SystemDidWakeUp += handler);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.TitleParametersDidChange"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="ActionEventArgs{TitlePayload}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.TitleParametersDidChange"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapTitleParametersDidChange(this IHost host, Delegate action)
            => host.MapEvent<ActionEventArgs<TitlePayload>>(action, (conn, handler) => conn.TitleParametersDidChange += handler, args => args.Context);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.WillAppear"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="ActionEventArgs{KeyPayload}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.WillAppear"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapWillAppear(this IHost host, Delegate action)
            => host.MapEvent<ActionEventArgs<ActionPayload>>(action, (conn, handler) => conn.WillAppear += handler, args => args.Context);

        /// <summary>
        /// Maps the delegate to the <see cref="IStreamDeckConnection.WillDisappear"/> event. When invoked, parameters are resolved from the host's <see cref="IServiceProvider"/>,
        /// except <see cref="IStreamDeckConnection"/> and <see cref="ActionEventArgs{ActionPayload}"/> which are propagated directly from the event.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action to map to <see cref="IStreamDeckConnection.WillDisappear"/>.</param>
        /// <returns>The same instance of the <see cref="IHost"/> for chaining.</returns>
        public static IHost MapWillDisappear(this IHost host, Delegate action)
            => host.MapEvent<ActionEventArgs<ActionPayload>>(action, (conn, handler) => conn.WillDisappear += handler, args => args.Context);

        /// <summary>
        /// Compiles the delegate, and invokes <paramref name="addHandler"/> allowing the event handler to be added to the <see cref="IStreamDeckConnection"/>.
        /// </summary>
        /// <typeparam name="TArgs">The type of the event arguments.</typeparam>
        /// <param name="host">The <see cref="IHost"/> to configure.</param>
        /// <param name="action">The action that will be invoked by the <see cref="IStreamDeckConnection"/>.</param>
        /// <param name="addHandler">The delegate responsible for mapping the delegate to the <see cref="IStreamDeckConnection"/>.</param>
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
