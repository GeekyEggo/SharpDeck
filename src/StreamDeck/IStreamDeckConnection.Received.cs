namespace StreamDeck
{
    using System.Text.Json.Nodes;
    using StreamDeck.Events;

    /// <inheritdoc/>
    public partial interface IStreamDeckConnection
    {
        /// <summary>
        /// Occurs when a monitored application is launched.
        /// </summary>
        event EventHandler<StreamDeckEventArgs<ApplicationPayload>>? ApplicationDidLaunch;

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// </summary>
        event EventHandler<StreamDeckEventArgs<ApplicationPayload>>? ApplicationDidTerminate;

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// </summary>
        event EventHandler<DeviceConnectEventArgs>? DeviceDidConnect;

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// </summary>
        event EventHandler<DeviceEventArgs>? DeviceDidDisconnect;

        /// <summary>
        /// Occurs when <see cref="GetGlobalSettingsAsync(CancellationToken)"/> has been called to retrieve the persistent global data stored for the plugin.
        /// </summary>
        event EventHandler<StreamDeckEventArgs<SettingsPayload>>? DidReceiveGlobalSettings;

        /// <summary>
        /// Occurs when <see cref="GetSettingsAsync(string, CancellationToken)"/> has been called to retrieve the persistent data stored for the action.
        /// </summary>
        event EventHandler<ActionEventArgs<ActionPayload>>? DidReceiveSettings;

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        event EventHandler<ActionEventArgs<KeyPayload>>? KeyDown;

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        event EventHandler<ActionEventArgs<KeyPayload>>? KeyUp;

        /// <summary>
        /// Occurs when the Property Inspector appears.
        /// </summary>
        event EventHandler<ActionEventArgs>? PropertyInspectorDidAppear;

        /// <summary>
        /// Occurs when the Property Inspector disappears
        /// </summary>
        event EventHandler<ActionEventArgs>? PropertyInspectorDidDisappear;

        /// <summary>
        /// Occurs when the property inspector sends a message to the plugin.
        /// </summary>
        event EventHandler<ActionEventArgs<JsonObject>>? SendToPlugin;

        /// <summary>
        /// Occurs when the computer is woken up.
        /// </summary>
        /// <remarks>
        /// A plugin may receive multiple <see cref="SystemDidWakeUp"/> events when waking up the computer.
        /// When the plugin receives the <see cref="SystemDidWakeUp"/> event, there is no garantee that the devices are available.
        /// </remarks>
        event EventHandler<StreamDeckEventArgs>? SystemDidWakeUp;

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        event EventHandler<ActionEventArgs<TitlePayload>>? TitleParametersDidChange;

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        event EventHandler<ActionEventArgs<AppearancePayload>>? WillAppear;

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        event EventHandler<ActionEventArgs<AppearancePayload>>? WillDisappear;
    }
}
