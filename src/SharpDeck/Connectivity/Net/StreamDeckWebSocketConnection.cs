namespace SharpDeck.Connectivity.Net
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using SharpDeck.Enums;
    using SharpDeck.Events.Received;
    using SharpDeck.Events.Sent;
    using SharpDeck.Extensions;

    /// <summary>
    /// Provides a connection between Elgato Stream Deck devices and a Stream Deck client.
    /// </summary>
    internal sealed class StreamDeckWebSocketConnection : IStreamDeckConnection
    {
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
        /// Occurs when <see cref="IStreamDeckConnection.GetGlobalSettingsAsync()"/> has been called to retrieve the persistent global data stored for the plugin.
        /// </summary>
        public event EventHandler<StreamDeckEventArgs<SettingsPayload>> DidReceiveGlobalSettings;

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.GetSettingsAsync(string)"/> has been called to retrieve the persistent data stored for the action.
        /// </summary>
        public event EventHandler<ActionEventArgs<ActionPayload>> DidReceiveSettings;

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        public event EventHandler<ActionEventArgs<KeyPayload>> KeyDown;

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        public event EventHandler<ActionEventArgs<KeyPayload>> KeyUp;

        /// <summary>
        /// Occurs when the Property Inspector appears.
        /// </summary>
        public event EventHandler<ActionEventArgs> PropertyInspectorDidAppear;

        /// <summary>
        /// Occurs when the Property Inspector disappears
        /// </summary>
        public event EventHandler<ActionEventArgs> PropertyInspectorDidDisappear;

        /// <summary>
        /// Occurs when the property inspector sends a message to the plugin.
        /// </summary>
        public event EventHandler<ActionEventArgs<JObject>> SendToPlugin;

        /// <summary>
        /// Occurs when the computer is woken up.
        /// </summary>
        /// <remarks>
        /// A plugin may receive multiple <see cref="SystemDidWakeUp"/> events when waking up the computer.
        /// When the plugin receives the <see cref="SystemDidWakeUp"/> event, there is no garantee that the devices are available.
        /// </remarks>
        public event EventHandler<StreamDeckEventArgs> SystemDidWakeUp;

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        public event EventHandler<ActionEventArgs<TitlePayload>> TitleParametersDidChange;

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        public event EventHandler<ActionEventArgs<AppearancePayload>> WillAppear;

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        public event EventHandler<ActionEventArgs<AppearancePayload>> WillDisappear;

        /// <summary>
        /// Gets the information about the connection.
        /// </summary>
        public RegistrationInfo Info => this.RegistrationParameters.Info;

        /// <summary>
        /// Gets the default JSON settings.
        /// </summary>
        private JsonSerializerSettings JsonSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.None
        };

        /// <summary>
        /// Gets or sets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; set; }

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        private WebSocketConnection WebSocket { get; set; }

        /// <summary>
        /// Initiates a connection to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task ConnectAsync(RegistrationParameters registrationParameters, CancellationToken cancellationToken)
        {
            this.RegistrationParameters = registrationParameters;

            this.WebSocket = new WebSocketConnection($"ws://localhost:{registrationParameters.Port}/", this.JsonSettings);
            this.WebSocket.MessageReceived += this.WebSocket_MessageReceived;

            return Task.Run(async () =>
            {
                await this.WebSocket.ConnectAsync();
                await this.WebSocket.SendJsonAsync(new RegistrationMessage(registrationParameters.Event, registrationParameters.PluginUUID));
                await this.WebSocket.ReceiveAsync(cancellationToken);
            });
        }

        /// <summary>
        /// Disconnects the connection asynchronously.
        /// </summary>
        /// <returns>The task of disconnecting</returns>
        public Task DisconnectAsync()
            => this.WebSocket.DisconnectAsync();


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.WebSocket?.Dispose();
            this.WebSocket = null;
        }

        /// <summary>
        /// Requests the persistent global data stored for the plugin.
        /// </summary>
        /// <returns>The task of sending the message; this result does not contain the settings.</returns>
        public Task GetGlobalSettingsAsync()
            => this.SendAsync(new ContextMessage("getGlobalSettings", this.RegistrationParameters.PluginUUID));

        /// <summary>
        /// Requests the persistent data stored for the specified context's action instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The task of sending the message; the result does not contain the settings.</returns>
        public Task GetSettingsAsync(string context)
            => this.SendAsync(new ContextMessage("getSettings", context));

        /// <summary>
        /// Write a debug log to the logs file.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <returns>The task of logging the message.</returns>
        public Task LogMessageAsync(string msg)
            => this.SendAsync(new Message<LogPayload>("logMessage", new LogPayload(msg)));

        /// <summary>
        /// Open a URL in the default browser.
        /// </summary>
        /// <param name="url">A URL to open in the default browser.</param>
        /// <returns>The task of opening the URL.</returns>
        public Task OpenUrlAsync(string url)
            => this.SendAsync(new Message<UrlPayload>("openUrl", new UrlPayload(url)));

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="context">An opaque value identifying the instances action.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        /// <returns>The task of sending payload to the property inspector.</returns>
        public Task SendToPropertyInspectorAsync(string context, string action, object payload)
            => this.SendAsync(new ActionMessage<object>("sendToPropertyInspector", context, action, payload));

        /// <summary>
        /// Save persistent data for the plugin.
        /// </summary>
        /// <param name="settings">An object which persistently saved globally.</param>
        /// <returns>The task of setting the global settings.</returns>
        public Task SetGlobalSettingsAsync(object settings)
            => this.SendAsync(new ContextMessage<object>("setGlobalSettings", this.RegistrationParameters.PluginUUID, JObject.FromObject(settings, JsonSerializer.Create(this.JsonSettings))));

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action; starting with Stream Deck 4.5.1, this API accepts svg images.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). svg is also supported. If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the image is set to all states.</param>
        /// <returns>The task of setting the image.</returns>
        public Task SetImageAsync(string context, string image, TargetType target = TargetType.Both, int? state = null)
            => this.SendAsync(new ContextMessage<SetImagePayload>("setImage", context, new SetImagePayload(image, target, state)));

        /// <summary>
        /// Save persistent data for the action's instance.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="settings">An object which is persistently saved for the action's instance.</param>
        /// <returns>The task of setting the settings.</returns>
        public Task SetSettingsAsync(string context, object settings)
            => this.SendAsync(new ContextMessage<object>("setSettings", context, JObject.FromObject(settings, JsonSerializer.Create(this.JsonSettings))));

        /// <summary>
        /// Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        /// <returns>The task of setting the state.</returns>
        public Task SetStateAsync(string context, int state = 0)
            => this.SendAsync(new ContextMessage<SetStatePayload>("setState", context, new SetStatePayload(state)));

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action you want to modify.</param>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <returns>The task of setting the title.</returns>
        public Task SetTitleAsync(string context, string title = "", TargetType target = TargetType.Both, int? state = null)
            => this.SendAsync(new ContextMessage<SetTitlePayload>("setTitle", context, new SetTitlePayload(title, target, state)));

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <returns>The task of showing the alert.</returns>
        public Task ShowAlertAsync(string context)
            => this.SendAsync(new ContextMessage("showAlert", context));

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <returns>The task of showing the OK.</returns>
        public Task ShowOkAsync(string context)
            => this.SendAsync(new ContextMessage("showOk", context));

        /// <summary>
        /// Switch to one of the preconfigured read-only profiles.
        /// </summary>
        /// <param name="context">An opaque value identifying the plugin. This value should be set to the PluginUUID received during the registration procedure.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="profile">The name of the profile to switch to. The name should be identical to the name provided in the manifest.json file.</param>
        /// <returns>The task of switching profiles.</returns>
        public Task SwitchToProfileAsync(string context, string device, string profile)
            => this.SendAsync(new DeviceMessage<SwitchToProfilePayload>("switchToProfile", context, device, new SwitchToProfilePayload(profile)));

        /// <summary>
        /// Sends the value to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The task of sending the value.</returns>
        private Task SendAsync(object value)
            => this.WebSocket.SendJsonAsync(value);

        /// <summary>
        /// Handles the <see cref="WebSocketConnection.MessageReceived"/> public event of <see cref="WebSocket"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs"/> instance containing the public event data.</param>
        private void WebSocket_MessageReceived(object sender, WebSocketMessageEventArgs e)
        {
            try
            {
                // attempt to parse the original message
                var args = JObject.Parse(e.Message);
                if (!args.TryGetString(nameof(StreamDeckEventArgs.Event), out var @event))
                {
                    throw new ArgumentException("Unable to parse public event from message");
                }

                // propagate the event, removing the sync context to prpublic event dead-locking
                this.Raise(@event, args);
            }
            catch (Exception ex)
            {
                _ = this.LogMessageAsync(ex.Message);
            }
        }

        /// <summary>
        /// Attempts to propagate the specified event.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The task of propagating the event.</returns>
        private void Raise(string @event, JObject args)
        {
            switch (@event)
            {
                // global
                case "applicationDidLaunch":
                    this.ApplicationDidLaunch?.Invoke(this, args.ToObject<StreamDeckEventArgs<ApplicationPayload>>());
                    break;

                case "applicationDidTerminate":
                    this.ApplicationDidTerminate?.Invoke(this, args.ToObject<StreamDeckEventArgs<ApplicationPayload>>());
                    break;

                case "deviceDidConnect":
                    this.DeviceDidConnect?.Invoke(this, args.ToObject<DeviceConnectEventArgs>());
                    break;

                case "deviceDidDisconnect":
                    this.DeviceDidDisconnect?.Invoke(this, args.ToObject<DeviceEventArgs>());
                    break;

                case "didReceiveGlobalSettings":
                    this.DidReceiveGlobalSettings?.Invoke(this, args.ToObject<StreamDeckEventArgs<SettingsPayload>>());
                    break;

                case "systemDidWakeUp":
                    this.SystemDidWakeUp?.Invoke(this, args.ToObject<StreamDeckEventArgs>());
                    break;

                // action specific
                case "didReceiveSettings":
                    this.DidReceiveSettings?.Invoke(this, args.ToObject<ActionEventArgs<ActionPayload>>());
                    break;

                case "keyDown":
                    this.KeyDown?.Invoke(this, args.ToObject<ActionEventArgs<KeyPayload>>());
                    break;

                case "keyUp":
                    this.KeyUp?.Invoke(this, args.ToObject<ActionEventArgs<KeyPayload>>());
                    break;

                case "propertyInspectorDidAppear":
                    this.PropertyInspectorDidAppear?.Invoke(this, args.ToObject<ActionEventArgs>());
                    break;

                case "propertyInspectorDidDisappear":
                    this.PropertyInspectorDidDisappear?.Invoke(this, args.ToObject<ActionEventArgs>());
                    break;

                case "sendToPlugin":
                    this.SendToPlugin?.Invoke(this, args.ToObject<ActionEventArgs<JObject>>());
                    break;

                case "titleParametersDidChange":
                    this.TitleParametersDidChange?.Invoke(this, args.ToObject<ActionEventArgs<TitlePayload>>());
                    break;

                case "willAppear":
                    this.WillAppear?.Invoke(this, args.ToObject<ActionEventArgs<AppearancePayload>>());
                    break;

                case "willDisappear":
                    this.WillDisappear?.Invoke(this, args.ToObject<ActionEventArgs<AppearancePayload>>());
                    break;

                // unrecognised
                default:
                    throw new ArgumentException($"Unrecognised event: {@event}", nameof(@event));
            }
        }
    }
}
