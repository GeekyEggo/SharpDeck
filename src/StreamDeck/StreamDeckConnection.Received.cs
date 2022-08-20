namespace StreamDeck
{
    using System.Text.Json.Nodes;
    using StreamDeck.Events;

    /// <inheritdoc/>
    public sealed partial class StreamDeckConnection
    {
        /// <inheritdoc/>
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>>? ApplicationDidLaunch;

        /// <inheritdoc/>
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>>? ApplicationDidTerminate;

        /// <inheritdoc/>
        public event EventHandler<DeviceConnectEventArgs>? DeviceDidConnect;

        /// <inheritdoc/>
        public event EventHandler<DeviceEventArgs>? DeviceDidDisconnect;

        /// <inheritdoc/>
        public event EventHandler<StreamDeckEventArgs<SettingsPayload>>? DidReceiveGlobalSettings;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<ActionPayload>>? DidReceiveSettings;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<KeyPayload>>? KeyDown;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<KeyPayload>>? KeyUp;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs>? PropertyInspectorDidAppear;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs>? PropertyInspectorDidDisappear;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<JsonObject>>? SendToPlugin;

        /// <inheritdoc/>
        public event EventHandler<StreamDeckEventArgs>? SystemDidWakeUp;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<TitlePayload>>? TitleParametersDidChange;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<AppearancePayload>>? WillAppear;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<AppearancePayload>>? WillDisappear;
    }
}
