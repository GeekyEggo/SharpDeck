namespace SharpDeck
{
    using Events;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using SharpDeck.Enums;
    using SharpDeck.Messages;
    using SharpDeck.Models;
    using SharpDeck.Net;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides events and methods that allow for communication with an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckClient : IDisposable
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
        /// <param name="regParams">The registration parameters.</param>
        public StreamDeckClient(RegistrationParameters regParams)
        {
            this.WebSocket = new ClientWebSocketWrapper($"ws://localhost:{regParams.Port}/");
            this.WebSocket.OnMessage += this.OnWebSocketClientMessage;
        }

        /// <summary>
        /// Occurs when the client encounters an error.
        /// </summary>
        public EventHandler<StreamDeckClientErrorEventArgs> OnError;

        /// <summary>
        /// Occurs when a monitored application is launched.
        /// </summary>
        [StreamDeckEvent("applicationDidLaunch")]
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidLaunch;

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// </summary>
        [StreamDeckEvent("applicationDidTerminate")]
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidTerminate;

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// </summary>
        [StreamDeckEvent("deviceDidConnect")]
        public event EventHandler<DeviceConnectEventArgs> DeviceDidConnect;

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// </summary>
        [StreamDeckEvent("deviceDidDisconnect")]
        public event EventHandler<DeviceEventArgs> DeciceDidDisconnect;

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        [StreamDeckEvent("keyDown")]
        public event EventHandler<ActionEventArgs<KeyPayload>> KeyDown;

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        [StreamDeckEvent("keyUp")]
        public event EventHandler<ActionEventArgs<KeyPayload>> KeyUp;

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        [StreamDeckEvent("titleParametersDidChange")]
        public event EventHandler<ActionEventArgs<TitlePayload>> TitleParametersDidChange;

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        [StreamDeckEvent("willAppear")]
        public event EventHandler<ActionEventArgs<ActionPayload>> WillAppear;

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        [StreamDeckEvent("willDisappear")]
        public event EventHandler<ActionEventArgs<ActionPayload>> WillDisappear;

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        internal IWebSocket WebSocket { get; }

        /// <summary>
        /// Gets or sets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; set; }

        /// <summary>
        /// Starts the client.
        /// </summary>
        public async void Start()
            => await this.WebSocket.ConnectAsync();

        /// <summary>
        /// Stops the client.
        /// </summary>
        public async void Stop()
            => await this.WebSocket.DisconnectAsync();

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
        /// <returns>The task.</returns>
        public Task SetTitleAsync(string context, string title = "", TargetType target = TargetType.Both)
            => this.SendMessageAsync(new ContextMessage<SetTitlePayload>("setTitle", context, new SetTitlePayload(title, target)));

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="base64Image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <returns>The task.</returns>
        public Task SetImageAsync(string context, string base64Image, TargetType target = TargetType.Both)
            => this.SendMessageAsync(new ContextMessage<SetImagePayload>("setImage", context, new SetImagePayload(base64Image, target)));

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <returns>The task.</returns>
        public Task ShowAlertAsync(string context)
            => this.SendMessageAsync(new ContextMessage("showAlert", context));

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <returns>The task.</returns>
        public Task ShowOkAsync(string context)
            => this.SendMessageAsync(new ContextMessage("showOk", context));

        /// <summary>
        /// Save persistent data for the actions instance.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="payload">A JSON object which is persistently saved for the action's instance.</param>
        /// <returns>The task.</returns>
        public Task SetSettings(string context, object payload)
            => this.SendMessageAsync(new ContextMessage<object>("setSettings", context, payload));

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        /// <returns>The task.</returns>
        public Task SetStateAsync(string context, int state = 0)
            => this.SendMessageAsync(new ContextMessage<SetStatePayload>("setState", context, new SetStatePayload(state)));

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="context">An opaque value identifying the instances action.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        /// <returns>The task.</returns>
        public Task SendToPropertyInspectorAsync(string context, string action, object payload)
            => this.SendMessageAsync(new ActionMessage<object>("sendToPropertyInspector", context, action, payload));

        /// <summary>
        /// Switch to one of the preconfigured read-only profiles.
        /// </summary>
        /// <param name="context">An opaque value identifying the plugin. This value should be set to the PluginUUID received during the registration procedure.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="profile">The name of the profile to switch to. The name should be identical to the name provided in the manifest.json file.</param>
        /// <returns>The task</returns>
        public Task SwitchToProfileAsync(string context, string device, string profile)
            => this.SendMessageAsync(new DeviceMessage<SwitchToProfilePayload>("switchToProfile", context, device, new SwitchToProfilePayload(profile)));

        /// <summary>
        /// Open a URL in the default browser.
        /// </summary>
        /// <param name="url">A URL to open in the default browser.</param>
        /// <returns></returns>
        public Task OpenUrlAsync(string url)
            => this.SendMessageAsync(new Message<UrlPayload>(url, new UrlPayload(url)));

        /// <summary>
        /// Called when <see cref="IWebSocket.OnMessage"/> is triggered.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnWebSocketClientMessage(object sender, WebSocketMessageEventArgs e)
        {
            try
            {
                if (StreamDeckEventFactory.TryParse(e, out var ev, out var args))
                {
                    ev.Invoke(this, args);
                    this.WebSocket.SendAsync("Thank you for the tasty msg");
                }
            }
            catch (Exception ex)
            {
                this.OnError?.Invoke(this, new StreamDeckClientErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// Sends the message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The task.</returns>
        private Task SendMessageAsync(object message)
        {
            string json = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.None
            });

            return this.WebSocket.SendAsync(json);
        }
    }
}
