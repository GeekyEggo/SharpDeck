namespace SharpDeck
{
    using Events;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Enums;
    using SharpDeck.Messages;
    using SharpDeck.Models;
    using SharpDeck.Net;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides events and methods that allow for communication with an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckClient : ActionEventHandler, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckClient"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public StreamDeckClient(string[] args)
            : this(RegistrationParameters.Parse(args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckClient"/> class.
        /// </summary>
        /// <param name="registrationParameters">The registration parameters.</param>
        public StreamDeckClient(RegistrationParameters registrationParameters)
        {
            this.EventRouter = new StreamDeckEventRouter(this);

            this.RegistrationParameters = registrationParameters;

            this.WebSocket = new ClientWebSocketWrapper($"ws://localhost:{registrationParameters.Port}/");
            this.WebSocket.MessageReceived += this.WebSocket_MessageReceived;
        }

        /// <summary>
        /// Occurs when the client encounters an error.
        /// </summary>
        public event EventHandler<StreamDeckClientErrorEventArgs> Error;

        /// <summary>
        /// Occurs when a monitored application is launched.
        /// </summary>
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidLaunch;

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// </summary>
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidTerminate;

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// </summary>
        public event EventHandler<DeviceConnectEventArgs> DeviceDidConnect;

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// </summary>
        public event EventHandler<DeviceEventArgs> DeviceDidDisconnect;

        /// <summary>
        /// Gets the event router.
        /// </summary>
        private StreamDeckEventRouter EventRouter { get; }

        /// <summary>
        /// Gets or sets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; set; }

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        private IWebSocket WebSocket { get; }

        /// <summary>
        /// Registers a Stream Deck Action.
        /// </summary>
        /// <typeparam name="T">The type of the action.</typeparam>
        /// <param name="actionUUID">The action UUID; this can be found in the manifest.json file.</param>
        public void RegisterAction<T>(string actionUUID)
            where T : StreamDeckAction
            => this.EventRouter.Register<T>(actionUUID);

        /// <summary>
        /// Starts Stream Deck client, and continuously listens for events received by the Elgato Stream Deck.
        /// </summary>
        public void Start()
            => this.Start(CancellationToken.None);

        /// <summary>
        /// Starts Stream Deck client, and continuously listens for events received by the Elgato Stream Deck.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Start(CancellationToken cancellationToken)
            => Task.WaitAll(this.StartAsync(cancellationToken));

        /// <summary>
        /// Starts Stream Deck client, and continuously listens for events received by the Elgato Stream Deck.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.WebSocket.ConnectAsync();
            await this.WebSocket.SendJsonAsync(new RegistrationMessage(this.RegistrationParameters.Event, this.RegistrationParameters.PluginUUID));
            await this.WebSocket.ReceiveAsync(cancellationToken);
        }

        /// <summary>
        /// Stops the client.
        /// </summary>
        public void Stop()
            => Task.WaitAll(this.WebSocket.DisconnectAsync());

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
            => this.WebSocket?.Dispose();

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action you want to modify.</param>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetTitleAsync(string context, string title = "", TargetType target = TargetType.Both)
            => this.WebSocket.SendJsonAsync(new ContextMessage<SetTitlePayload>("setTitle", context, new SetTitlePayload(title, target)));

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="base64Image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetImageAsync(string context, string base64Image, TargetType target = TargetType.Both)
            => this.WebSocket.SendJsonAsync(new ContextMessage<SetImagePayload>("setImage", context, new SetImagePayload(base64Image, target)));

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        public Task ShowAlertAsync(string context)
            => this.WebSocket.SendJsonAsync(new ContextMessage("showAlert", context));

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        public Task ShowOkAsync(string context)
            => this.WebSocket.SendJsonAsync(new ContextMessage("showOk", context));

        /// <summary>
        /// Save persistent data for the actions instance.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        public Task SetSettingsAsync(string context, object settings)
            => this.WebSocket.SendJsonAsync(new ContextMessage<object>("setSettings", context, JObject.FromObject(settings)));

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        public Task SetStateAsync(string context, int state = 0)
            => this.WebSocket.SendJsonAsync(new ContextMessage<SetStatePayload>("setState", context, new SetStatePayload(state)));

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="context">An opaque value identifying the instances action.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        public Task SendToPropertyInspectorAsync(string context, string action, object payload)
            => this.WebSocket.SendJsonAsync(new ActionMessage<object>("sendToPropertyInspector", context, action, payload));

        /// <summary>
        /// Switch to one of the preconfigured read-only profiles.
        /// </summary>
        /// <param name="context">An opaque value identifying the plugin. This value should be set to the PluginUUID received during the registration procedure.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="profile">The name of the profile to switch to. The name should be identical to the name provided in the manifest.json file.</param>
        public Task SwitchToProfileAsync(string context, string device, string profile)
            => this.WebSocket.SendJsonAsync(new DeviceMessage<SwitchToProfilePayload>("switchToProfile", context, device, new SwitchToProfilePayload(profile)));

        /// <summary>
        /// Open a URL in the default browser.
        /// </summary>
        /// <param name="url">A URL to open in the default browser.</param>
        public Task OpenUrlAsync(string url)
            => this.WebSocket.SendJsonAsync(new Message<UrlPayload>(url, new UrlPayload(url)));

        /// <summary>
        /// Occurs when a monitored application is launched.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs{ApplicationPayload}"/> instance containing the event data.</param>
        [StreamDeckEvent("applicationDidLaunch")]
        protected virtual void OnApplicationDidLaunch(StreamDeckEventArgs<ApplicationPayload> args)
            => this.ApplicationDidLaunch?.Invoke(this, args);

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs{ApplicationPayload}"/> instance containing the event data.</param>
        [StreamDeckEvent("applicationDidTerminate")]
        protected virtual void OnApplicationDidTerminate(StreamDeckEventArgs<ApplicationPayload> args)
            => this.ApplicationDidTerminate?.Invoke(this, args);

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// </summary>
        /// <param name="args">The <see cref="DeviceConnectEventArgs"/> instance containing the event data.</param>
        [StreamDeckEvent("deviceDidConnect")]
        protected virtual void OnDeviceDidConnect(DeviceConnectEventArgs args)
            => this.DeviceDidConnect?.Invoke(this, args);

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// </summary>
        /// <param name="args">The <see cref="DeviceEventArgs"/> instance containing the event data.</param>
        [StreamDeckEvent("deviceDidDisconnect")]
        protected virtual void OnDeviceDidDisconnect(DeviceEventArgs args)
            => this.DeviceDidDisconnect?.Invoke(this, args);

        /// <summary>
        /// Handles the <see cref="IWebSocket.MessageReceived"/> event; triggering any associated events.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs"/> instance containing the event data.</param>
        private void WebSocket_MessageReceived(object sender, WebSocketMessageEventArgs e)
        {
            try
            {
                this.EventRouter.Route(e);
            }
            catch (Exception ex)
            {
                this.Error?.Invoke(this, new StreamDeckClientErrorEventArgs(ex.Message));
            }
        }
    }
}
