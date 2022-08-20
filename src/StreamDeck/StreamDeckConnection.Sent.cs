namespace StreamDeck
{
    using StreamDeck.Payloads;

    /// <inheritdoc/>
    public sealed partial class StreamDeckConnection
    {
        /// <inheritdoc/>
        public Task GetGlobalSettingsAsync(CancellationToken cancellationToken = default)
        {
            if (this.RegistrationParameters?.PluginUUID == null)
            {
                throw new NullReferenceException("Unable to get global settings as the pluginUUID is null.");
            }

            return this.SendAsync(new ContextMessage("getGlobalSettings", this.RegistrationParameters.PluginUUID), cancellationToken);
        }

        /// <inheritdoc/>
        public Task GetSettingsAsync(string context, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage("getSettings", context), cancellationToken);

        /// <inheritdoc/>
        public Task LogMessageAsync(string msg, CancellationToken cancellationToken = default)
            => this.SendAsync(new Message<LogPayload>("logMessage", new LogPayload(msg)), cancellationToken);

        /// <inheritdoc/>
        public Task OpenUrlAsync(string url, CancellationToken cancellationToken = default)
            => this.SendAsync(new Message<UrlPayload>("openUrl", new UrlPayload(url)), cancellationToken);

        /// <inheritdoc/>
        public Task SendToPropertyInspectorAsync(string context, string action, object payload, CancellationToken cancellationToken = default)
            => this.SendAsync(new ActionMessage<object>("sendToPropertyInspector", context, action, payload), cancellationToken);

        /// <inheritdoc/>
        public Task SetGlobalSettingsAsync(object settings, CancellationToken cancellationToken = default)
        {
            if (this.RegistrationParameters?.PluginUUID == null)
            {
                throw new NullReferenceException("Unable to set global settings as the pluginUUID is null.");
            }

            return this.SendAsync(new ContextMessage<object>("setGlobalSettings", this.RegistrationParameters.PluginUUID, settings), cancellationToken);
        }

        /// <inheritdoc/>
        public Task SetImageAsync(string context, string image = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage<SetImagePayload>("setImage", context, new SetImagePayload(image, target, state)), cancellationToken);

        /// <inheritdoc/>
        public Task SetSettingsAsync(string context, object settings, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage<object>("setSettings", context, settings), cancellationToken);

        /// <inheritdoc/>
        public Task SetStateAsync(string context, int state = 0, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage<SetStatePayload>("setState", context, new SetStatePayload(state)), cancellationToken);

        /// <inheritdoc/>
        public Task SetTitleAsync(string context, string title = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage<SetTitlePayload>("setTitle", context, new SetTitlePayload(title, target, state)), cancellationToken);

        /// <inheritdoc/>
        public Task ShowAlertAsync(string context, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage("showAlert", context), cancellationToken);

        /// <inheritdoc/>
        public Task ShowOkAsync(string context, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage("showOk", context), cancellationToken);

        /// <inheritdoc/>
        public Task SwitchToProfileAsync(string context, string device, string profile = "", CancellationToken cancellationToken = default)
            => this.SendAsync(new DeviceMessage<SwitchToProfilePayload>("switchToProfile", context, device, new SwitchToProfilePayload(profile)), cancellationToken);
    }
}
