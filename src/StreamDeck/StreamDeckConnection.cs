namespace StreamDeck
{
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using Microsoft.Extensions.Logging;
    using StreamDeck.Events;
    using StreamDeck.Extensions;
    using StreamDeck.Net;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides a connection between Elgato Stream Deck devices and a Stream Deck client.
    /// </summary>
    public sealed partial class StreamDeckConnection : IStreamDeckConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckConnection"/> class.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <param name="logger">The logger.</param>
        public StreamDeckConnection(string[]? args = null, ILogger<StreamDeckConnection>? logger = null)
            : this(new RegistrationParameters(args ?? Environment.GetCommandLineArgs()), new WebSocketConnection(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckConnection"/> class.
        /// </summary>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="webSocket">The web socket connection.</param>
        /// <param name="logger">The logger.</param>
        internal StreamDeckConnection(RegistrationParameters registrationParameters, IWebSocketConnection? webSocket = null, ILogger<StreamDeckConnection>? logger = null)
        {
            this.Logger = logger;
            this.RegistrationParameters = registrationParameters;

            this.WebSocket = webSocket ?? new WebSocketConnection();
            this.WebSocket.MessageReceived += this.WebSocket_MessageReceived;
        }

        /// <inheritdoc/>
        public RegistrationInfo Info => this.RegistrationParameters.Info;

        /// <summary>
        /// Gets the underlying the web socket.
        /// </summary>
        internal IWebSocketConnection WebSocket { get; }

        /// <summary>
        /// Gets or sets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger<StreamDeckConnection>? Logger { get; }

        /// <summary>
        /// Connects to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optioanl cancellation token.</param>
        /// <returns>The task of connecting to the Stream Deck.</returns>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("Connecting to Stream Deck.");
            await this.WebSocket.ConnectAsync($"ws://localhost:{this.RegistrationParameters.Port}/", cancellationToken);

            this.Logger?.LogTrace($"Registering plugin.");
            await this.WebSocket.SendAsync(this.RegistrationParameters, StreamDeckJsonContext.Default.RegistrationParameters, cancellationToken);

            this.Logger?.LogTrace($"Successfully connected to Stream Deck.");
        }

        /// <summary>
        /// Connects to the Stream Deck, and awaits disconnection asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        public async Task ConnectAndWaitAsync(CancellationToken cancellationToken = default)
        {
            await this.ConnectAsync(cancellationToken);
            await this.WebSocket.WaitForDisconnectAsync(cancellationToken);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.WebSocket?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await this.WebSocket.DisconnectAsync();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handles the <see cref="WebSocketConnection.MessageReceived"/> public event of <see cref="WebSocket"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs"/> instance containing the public event data.</param>
        private void WebSocket_MessageReceived(object? sender, WebSocketMessageEventArgs e)
        {
            try
            {
                var args = JsonSerializer.Deserialize<StreamDeckEventArgs>(e.Message, StreamDeckJsonContext.Default.Options);
                switch (args?.Event)
                {
                    // Global.
                    case "applicationDidLaunch":
                        this.ApplicationDidLaunch?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.StreamDeckEventArgsApplicationPayload));
                        break;

                    case "applicationDidTerminate":
                        this.ApplicationDidTerminate?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.StreamDeckEventArgsApplicationPayload));
                        break;

                    case "deviceDidConnect":
                        this.DeviceDidConnect?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.DeviceConnectEventArgs));
                        break;

                    case "deviceDidDisconnect":
                        this.DeviceDidDisconnect?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.DeviceEventArgs));
                        break;

                    case "didReceiveGlobalSettings":
                        this.DidReceiveGlobalSettings?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.StreamDeckEventArgsSettingsPayload));
                        break;

                    case "systemDidWakeUp":
                        this.SystemDidWakeUp?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.StreamDeckEventArgs));
                        break;

                    // Action specific.
                    case "didReceiveSettings":
                        this.DidReceiveSettings?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsActionPayload));
                        break;

                    case "keyDown":
                        this.KeyDown?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsKeyPayload));
                        break;

                    case "keyUp":
                        this.KeyUp?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsKeyPayload));
                        break;

                    case "propertyInspectorDidAppear":
                        this.PropertyInspectorDidAppear?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgs));
                        break;

                    case "propertyInspectorDidDisappear":
                        this.PropertyInspectorDidDisappear?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgs));
                        break;

                    case "sendToPlugin":
                        this.SendToPlugin?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsJsonObject));
                        break;

                    case "titleParametersDidChange":
                        this.TitleParametersDidChange?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsTitlePayload));
                        break;

                    case "willAppear":
                        this.WillAppear?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsActionPayload));
                        break;

                    case "willDisappear":
                        this.WillDisappear?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsActionPayload));
                        break;

                    // Unrecognised
                    default:
                        throw new InvalidOperationException($"Unrecognised event: {args?.Event ?? "[undefined]"}");
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "Exception encountered whilst handling Stream Deck event.");
            }
        }
    }
}
