namespace SharpDeck.Connectivity
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Events;
    using SharpDeck.Events.Received;
    using SharpDeck.Events.Sent;
    using SharpDeck.Exceptions;
    using SharpDeck.Extensions;
    using SharpDeck.Threading;

    /// <summary>
    /// Provides a connection between Elgato Stream Deck devices and a Stream Deck client.
    /// </summary>
    internal sealed class WebSocketStreamDeckConnection : StreamDeckEventPropagator, IStreamDeckConnection, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketStreamDeckConnection"/> class.
        /// </summary>
        public WebSocketStreamDeckConnection()
            : base()
        {
        }

        /// <summary>
        /// Occurs when a non-fatal exception is thrown, or an error is encountered.
        /// </summary>
        public event EventHandler<StreamDeckConnectionErrorEventArgs> Error;

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        private WebSocketConnection WebSocket { get; set; }

        /// <summary>
        /// Initiates a connection to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task ConnectAsync(RegistrationParameters registrationParameters, CancellationToken cancellationToken)
        {
            this.WebSocket = new WebSocketConnection($"ws://localhost:{registrationParameters.Port}/", Constants.DEFAULT_JSON_SETTINGS);
            this.WebSocket.MessageReceived += this.WebSocket_MessageReceived;

            await this.WebSocket.ConnectAsync();
            await this.WebSocket.SendJsonAsync(new RegistrationMessage(registrationParameters.Event, registrationParameters.PluginUUID));
            await this.WebSocket.ReceiveAsync(cancellationToken);
        }

        /// <summary>
        /// Disconnects the connection asynchronously.
        /// </summary>
        /// <returns>The task of disconnecting</returns>
        public Task DisconnectAsync()
            => this.WebSocket.DisconnectAsync();

        /// <summary>
        /// Sends the value to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The task of sending the value.</returns>
        public Task SendAsync(object value)
            => this.WebSocket.SendJsonAsync(value);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.WebSocket?.Dispose();
            this.WebSocket = null;
        }

        /// <summary>
        /// Handles the <see cref="WebSocketConnection.MessageReceived"/> event of <see cref="WebSocket"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs"/> instance containing the event data.</param>
        private void WebSocket_MessageReceived(object sender, WebSocketMessageEventArgs e)
        {
            var @event = string.Empty;
            var args = new JObject();

            try
            {
                // attempt to parse the original message
                args = JObject.Parse(e.Message);
                if (!args.TryGetString(nameof(StreamDeckEventArgs.Event), out @event))
                {
                    throw new ArgumentException("Unable to parse event from message");
                }

                // propagate the event, removing the sync context to prevent dead-locking
                using (SynchronizationContextSwitcher.NoContext())
                {
                    this.Propagate(@event, args);
                }
            }
            catch (Exception ex)
            {
                this.Error?.Invoke(this, new StreamDeckConnectionErrorEventArgs(@event, args, e.Message, ex));
            }
        }

        /// <summary>
        /// Attempts to propagate the specified event.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The task of propagating the event.</returns>
        private Task Propagate(string @event, JObject args)
        {
            switch (@event)
            {
                // global
                case "applicationDidLaunch":
                    return this.OnApplicationDidLaunch(args.ToObject<StreamDeckEventArgs<ApplicationPayload>>());
                case "applicationDidTerminate":
                    return this.OnApplicationDidTerminate(args.ToObject<StreamDeckEventArgs<ApplicationPayload>>());
                case "deviceDidConnect":
                    return this.OnDeviceDidConnect(args.ToObject<DeviceConnectEventArgs>());
                case "deviceDidDisconnect":
                    return this.OnDeviceDidDisconnect(args.ToObject<DeviceEventArgs>());
                case "didReceiveGlobalSettings":
                    return this.OnDidReceiveGlobalSettings(args.ToObject<StreamDeckEventArgs<SettingsPayload>>());
                case "systemDidWakeUp":
                    return this.OnSystemDidWakeUp(args.ToObject<StreamDeckEventArgs>());

                // action specific
                case "didReceiveSettings":
                    return this.OnDidReceiveSettings(args.ToObject<ActionEventArgs<ActionPayload>>());
                case "keyDown":
                    return this.OnKeyDown(args.ToObject<ActionEventArgs<KeyPayload>>());
                case "keyUp":
                    return this.OnKeyUp(args.ToObject<ActionEventArgs<KeyPayload>>());
                case "propertyInspectorDidAppear":
                    return this.OnPropertyInspectorDidAppear(args.ToObject<ActionEventArgs>());
                case "propertyInspectorDidDisappear":
                    return this.OnPropertyInspectorDidDisappear(args.ToObject<ActionEventArgs>());
                case "sendToPlugin":
                    return this.OnSendToPlugin(args.ToObject<ActionEventArgs<JObject>>());
                case "titleParametersDidChange":
                    return this.OnTitleParametersDidChange(args.ToObject<ActionEventArgs<TitlePayload>>());
                case "willAppear":
                    return this.OnWillAppear(args.ToObject<ActionEventArgs<AppearancePayload>>());
                case "willDisappear":
                    return this.OnWillDisappear(args.ToObject<ActionEventArgs<AppearancePayload>>());

                // unrecognised
                default:
                    throw new ArgumentException($"Unrecognised event: {@event}", nameof(@event));
            }
        }
    }
}
