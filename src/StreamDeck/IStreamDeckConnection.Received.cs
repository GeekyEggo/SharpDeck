namespace StreamDeck
{
    using System.Text.Json.Nodes;
    using StreamDeck.Events;

    /// <inheritdoc/>
    public partial interface IStreamDeckConnection
    {
        /// <summary>
        /// Occurs when a monitored application is launched.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#applicationdidlaunch"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, StreamDeckEventArgs<ApplicationPayload>?>? ApplicationDidLaunch;

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#applicationdidterminate"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, StreamDeckEventArgs<ApplicationPayload>?>? ApplicationDidTerminate;

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#devicedidconnect"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, DeviceConnectEventArgs?>? DeviceDidConnect;

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#devicediddisconnect"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, DeviceEventArgs?>? DeviceDidDisconnect;

        /// <summary>
        /// Occurs when <see cref="GetGlobalSettingsAsync(CancellationToken)"/> has been called to retrieve the persistent global data stored for the plugin.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceiveglobalsettings"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, StreamDeckEventArgs<SettingsPayload>?>? DidReceiveGlobalSettings;

        /// <summary>
        /// Occurs when <see cref="GetSettingsAsync(string, CancellationToken)"/> has been called to retrieve the persistent data stored for the action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceivesettings"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, ActionEventArgs<ActionPayload>?>? DidReceiveSettings;

        /// <summary>
        /// Occurs when the user presses a key.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#keydown"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, ActionEventArgs<KeyPayload>?>? KeyDown;

        /// <summary>
        /// Occurs when the user releases a key.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#keyup"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, ActionEventArgs<KeyPayload>?>? KeyUp;

        /// <summary>
        /// Occurs when the Property Inspector appears.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#propertyinspectordidappear"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, ActionEventArgs?>? PropertyInspectorDidAppear;

        /// <summary>
        /// Occurs when the Property Inspector disappears
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#propertyinspectordiddisappear"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, ActionEventArgs?>? PropertyInspectorDidDisappear;

        /// <summary>
        /// Occurs when the property inspector sends a message to the plugin.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#sendtoplugin"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, PartialActionEventArgs<JsonObject>?>? SendToPlugin;

        /// <summary>
        /// Occurs when the computer is woken up.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#systemdidwakeup"/>.
        /// </summary>
        /// <remarks>
        /// A plugin may receive multiple <see cref="SystemDidWakeUp"/> events when waking up the computer.
        /// When the plugin receives the <see cref="SystemDidWakeUp"/> event, there is no garantee that the devices are available.
        /// </remarks>
        event EventHandler<IStreamDeckConnection, StreamDeckEventArgs?>? SystemDidWakeUp;

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#titleparametersdidchange"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, ActionEventArgs<TitlePayload>?>? TitleParametersDidChange;

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#willappear"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, ActionEventArgs<ActionPayload>?>? WillAppear;

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#willdisappear"/>.
        /// </summary>
        event EventHandler<IStreamDeckConnection, ActionEventArgs<ActionPayload>?>? WillDisappear;
    }
}
