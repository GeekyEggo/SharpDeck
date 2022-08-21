namespace StreamDeck
{
    using StreamDeck.Extensions;
    using StreamDeck.Payloads;
    using StreamDeck.Serialization;

    /// <inheritdoc/>
    public sealed partial class StreamDeckConnection : IStreamDeckConnection
    {
        /// <inheritdoc/>
        public Task GetGlobalSettingsAsync(CancellationToken cancellationToken = default)
        {
            if (this.RegistrationParameters?.PluginUUID == null)
            {
                throw new NullReferenceException("Unable to get global settings as the pluginUUID is null.");
            }

            return this.WebSocket.SendAsync(new ContextMessage("getGlobalSettings", this.RegistrationParameters.PluginUUID), StreamDeckJsonContext.Default.ContextMessage, cancellationToken);
        }

        /// <inheritdoc/>
        public Task GetSettingsAsync(string context, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new ContextMessage("getSettings", context), StreamDeckJsonContext.Default.ContextMessage, cancellationToken);

        /// <inheritdoc/>
        public Task LogMessageAsync(string msg, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new Message<LogPayload>("logMessage", new LogPayload(msg)), StreamDeckJsonContext.Default.MessageLogPayload, cancellationToken);

        /// <inheritdoc/>
        public Task OpenUrlAsync(string url, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new Message<UrlPayload>("openUrl", new UrlPayload(url)), StreamDeckJsonContext.Default.MessageUrlPayload, cancellationToken);

        /// <inheritdoc/>
        public Task SendToPropertyInspectorAsync(string context, string action, object payload, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new ActionMessage<object>("sendToPropertyInspector", context, action, payload), StreamDeckJsonContext.Default.ActionMessageObject, cancellationToken);

        /// <inheritdoc/>
        public Task SetGlobalSettingsAsync(object settings, CancellationToken cancellationToken = default)
        {
            if (this.RegistrationParameters?.PluginUUID == null)
            {
                throw new NullReferenceException("Unable to set global settings as the pluginUUID is null.");
            }

            return this.WebSocket.SendAsync(new ContextMessage<object>("setGlobalSettings", this.RegistrationParameters.PluginUUID, settings), StreamDeckJsonContext.Default.ContextMessageObject, cancellationToken);
        }

        /// <inheritdoc/>
        public Task SetImageAsync(string context, string image = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new ContextMessage<SetImagePayload>("setImage", context, new SetImagePayload(image, target, state)), StreamDeckJsonContext.Default.ContextMessageSetImagePayload, cancellationToken);

        /// <inheritdoc/>
        public Task SetSettingsAsync(string context, object settings, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new ContextMessage<object>("setSettings", context, settings), StreamDeckJsonContext.Default.ContextMessageObject, cancellationToken);

        /// <inheritdoc/>
        public Task SetStateAsync(string context, int state = 0, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new ContextMessage<SetStatePayload>("setState", context, new SetStatePayload(state)), StreamDeckJsonContext.Default.ContextMessageSetStatePayload, cancellationToken);

        /// <inheritdoc/>
        public Task SetTitleAsync(string context, string title = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new ContextMessage<SetTitlePayload>("setTitle", context, new SetTitlePayload(title, target, state)), StreamDeckJsonContext.Default.ContextMessageSetTitlePayload, cancellationToken);

        /// <inheritdoc/>
        public Task ShowAlertAsync(string context, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new ContextMessage("showAlert", context), StreamDeckJsonContext.Default.ContextMessage, cancellationToken);

        /// <inheritdoc/>
        public Task ShowOkAsync(string context, CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new ContextMessage("showOk", context), StreamDeckJsonContext.Default.ContextMessage, cancellationToken);

        /// <inheritdoc/>
        public Task SwitchToProfileAsync(string context, string device, string profile = "", CancellationToken cancellationToken = default)
            => this.WebSocket.SendAsync(new DeviceMessage<SwitchToProfilePayload>("switchToProfile", context, device, new SwitchToProfilePayload(profile)), StreamDeckJsonContext.Default.DeviceMessageSwitchToProfilePayload, cancellationToken);
    }
}
