namespace StreamDeck
{
    using System.Text.Json.Nodes;
    using StreamDeck.Events;

    /// <inheritdoc/>
    public sealed partial class StreamDeckConnection
    {
        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, StreamDeckEventArgs<ApplicationPayload>>? ApplicationDidLaunch;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, StreamDeckEventArgs<ApplicationPayload>>? ApplicationDidTerminate;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, DeviceConnectEventArgs>? DeviceDidConnect;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, DeviceEventArgs>? DeviceDidDisconnect;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, StreamDeckEventArgs<SettingsPayload>>? DidReceiveGlobalSettings;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, ActionEventArgs<ActionPayload>>? DidReceiveSettings;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, ActionEventArgs<KeyPayload>>? KeyDown;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, ActionEventArgs<KeyPayload>>? KeyUp;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, ActionEventArgs>? PropertyInspectorDidAppear;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, ActionEventArgs>? PropertyInspectorDidDisappear;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, PartialActionEventArgs<JsonObject?>>? SendToPlugin;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, StreamDeckEventArgs>? SystemDidWakeUp;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, ActionEventArgs<TitlePayload>>? TitleParametersDidChange;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, ActionEventArgs<ActionPayload>>? WillAppear;

        /// <inheritdoc/>
        public event EventHandler<IStreamDeckConnection, ActionEventArgs<ActionPayload>>? WillDisappear;
    }
}
