[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SharpDeck.Tests")]

namespace SharpDeck
{
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using SharpDeck.Enums;
    using SharpDeck.Events.Received;
    using SharpDeck.Events.Sent;
    using SharpDeck.Exceptions;
    using SharpDeck.Net;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides events and methods that allow for communication with an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckClient : StreamDeckActionEventReceiver, IStreamDeckReceiver, IStreamDeckSender, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckClient"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="logger">The optional logger.</param>
        public StreamDeckClient(string[] args, ILogger logger = null)
            : this(RegistrationParameters.Parse(args), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckClient" /> class.
        /// </summary>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="logger">The optional logger.</param>
        public StreamDeckClient(RegistrationParameters registrationParameters, ILogger logger = null)
        {
            this.EventRouter = new StreamDeckEventRouter(logger);
            this.RegistrationParameters = registrationParameters;

            this.WebSocket = new ClientWebSocketWrapper($"ws://localhost:{registrationParameters.Port}/", DEFAULT_JSON_SETTINGS);
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
        /// Occurs when <see cref="IStreamDeckSender.GetGlobalSettingsAsync()"/> has been called to retrieve the persistent global data stored for the plugin.
        /// </summary>
        public event EventHandler<StreamDeckEventArgs<SettingsPayload>> DidReceiveGlobalSettings;

        /// <summary>
        /// Occurs when the computer is woken up.
        /// </summary>
        /// <remarks>
        /// A plugin may receive multiple <see cref="SystemDidWakeUp"/> events when waking up the computer.
        /// When the plugin receives the <see cref="SystemDidWakeUp"/> event, there is no garantee that the devices are available.
        /// </remarks>
        public event EventHandler<StreamDeckEventArgs> SystemDidWakeUp;

        /// <summary>
        /// Gets the default JSON settings.
        /// </summary>
        internal static JsonSerializerSettings DEFAULT_JSON_SETTINGS { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.None
        };

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
        /// Registers a new <see cref="StreamDeckAction"/> for the specified action UUID. When <typeparamref name="T"/> does not have a default constructor, consider specifying a `valueFactory`.
        /// </summary>
        /// <typeparam name="T">The type of Stream Deck action.</typeparam>
        /// <param name="actionUUID">The action UUID.</param>
        public void RegisterAction<T>(string actionUUID)
            where T : StreamDeckAction, new()
            => this.RegisterAction<T>(actionUUID, _ => new T());

        /// <summary>
        /// Registers a new <see cref="StreamDeckAction"/> for the specified action UUID.
        /// </summary>
        /// <typeparam name="T">The type of Stream Deck action.</typeparam>
        /// <param name="actionUUID">The action UUID.</param>
        /// <param name="valueFactory">The value factory, used to initialize a new action.</param>
        public void RegisterAction<T>(string actionUUID, Func<T> valueFactory)
            where T : StreamDeckAction
            => this.EventRouter.Register(actionUUID, _ => valueFactory());

        /// <summary>
        /// Registers a new <see cref="StreamDeckAction"/> for the specified action UUID.
        /// </summary>
        /// <typeparam name="T">The type of Stream Deck action.</typeparam>
        /// <param name="actionUUID">The action UUID.</param>
        /// <param name="valueFactory">The value factory, used to initialize a new action.</param>
        public void RegisterAction<T>(string actionUUID, Func<ActionEventArgs<AppearancePayload>, T> valueFactory)
            where T : StreamDeckAction
            => this.EventRouter.Register(actionUUID, valueFactory);

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
        {
            var task = this.StartAsync(cancellationToken);
            task.ConfigureAwait(false);
            task.Wait(cancellationToken);
        }

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
        /// Requests the persistent global data stored for the plugin.
        /// </summary>
        /// <returns>The task of sending the message; this result does not contain the settings.</returns>
        public Task GetGlobalSettingsAsync()
            => this.WebSocket.SendJsonAsync(new ContextMessage("getGlobalSettings", this.RegistrationParameters.PluginUUID));

        /// <summary>
        /// Requests the persistent data stored for the specified context's action instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The task of sending the message; the result does not contain the settings.</returns>
        public Task GetSettingsAsync(string context)
            => this.WebSocket.SendJsonAsync(new ContextMessage("getSettings", context));

        /// <summary>
        /// Write a debug log to the logs file.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        public Task LogMessageAsync(string msg)
            => this.WebSocket.SendJsonAsync(new Message<LogPayload>("logMessage", new LogPayload(msg)));

        /// <summary>
        /// Open a URL in the default browser.
        /// </summary>
        /// <param name="url">A URL to open in the default browser.</param>
        public Task OpenUrlAsync(string url)
            => this.WebSocket.SendJsonAsync(new Message<UrlPayload>("openUrl", new UrlPayload(url)));

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="context">An opaque value identifying the instances action.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        public Task SendToPropertyInspectorAsync(string context, string action, object payload)
            => this.WebSocket.SendJsonAsync(new ActionMessage<object>("sendToPropertyInspector", context, action, payload));

        /// <summary>
        /// Save persistent data for the plugin.
        /// </summary>
        /// <param name="settings">An object which persistently saved globally.</param>
        public Task SetGlobalSettingsAsync(object settings)
            => this.WebSocket.SendJsonAsync(new ContextMessage<object>("setGlobalSettings", this.RegistrationParameters.PluginUUID, JObject.FromObject(settings, JsonSerializer.Create(DEFAULT_JSON_SETTINGS))));

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="base64Image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetImageAsync(string context, string base64Image, TargetType target = TargetType.Both)
            => this.WebSocket.SendJsonAsync(new ContextMessage<SetImagePayload>("setImage", context, new SetImagePayload(base64Image, target)));

        /// <summary>
        /// Save persistent data for the action's instance.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="settings">An object which is persistently saved for the action's instance.</param>
        public Task SetSettingsAsync(string context, object settings)
            => this.WebSocket.SendJsonAsync(new ContextMessage<object>("setSettings", context, JObject.FromObject(settings, JsonSerializer.Create(DEFAULT_JSON_SETTINGS))));

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        public Task SetStateAsync(string context, int state = 0)
            => this.WebSocket.SendJsonAsync(new ContextMessage<SetStatePayload>("setState", context, new SetStatePayload(state)));

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action you want to modify.</param>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetTitleAsync(string context, string title = "", TargetType target = TargetType.Both)
            => this.WebSocket.SendJsonAsync(new ContextMessage<SetTitlePayload>("setTitle", context, new SetTitlePayload(title, target)));

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
        /// Switch to one of the preconfigured read-only profiles.
        /// </summary>
        /// <param name="context">An opaque value identifying the plugin. This value should be set to the PluginUUID received during the registration procedure.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="profile">The name of the profile to switch to. The name should be identical to the name provided in the manifest.json file.</param>
        public Task SwitchToProfileAsync(string context, string device, string profile)
            => this.WebSocket.SendJsonAsync(new DeviceMessage<SwitchToProfilePayload>("switchToProfile", context, device, new SwitchToProfilePayload(profile)));

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.EventRouter?.Dispose();
            this.WebSocket?.Dispose();
        }

        /// <summary>
        /// Occurs when a monitored application is launched.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs{ApplicationPayload}"/> instance containing the event data.</param>
        [StreamDeckEvent("applicationDidLaunch")]
        protected virtual Task OnApplicationDidLaunch(StreamDeckEventArgs<ApplicationPayload> args)
        {
            this.ApplicationDidLaunch?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs{ApplicationPayload}"/> instance containing the event data.</param>
        [StreamDeckEvent("applicationDidTerminate")]
        protected virtual Task OnApplicationDidTerminate(StreamDeckEventArgs<ApplicationPayload> args)
        {
            this.ApplicationDidTerminate?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// </summary>
        /// <param name="args">The <see cref="DeviceConnectEventArgs"/> instance containing the event data.</param>
        [StreamDeckEvent("deviceDidConnect")]
        protected virtual Task OnDeviceDidConnect(DeviceConnectEventArgs args)
        {
            this.DeviceDidConnect?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// </summary>
        /// <param name="args">The <see cref="DeviceEventArgs"/> instance containing the event data.</param>
        [StreamDeckEvent("deviceDidDisconnect")]
        protected virtual Task OnDeviceDidDisconnect(DeviceEventArgs args)
        {
            this.DeviceDidDisconnect?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Raises the <see cref="DidReceiveGlobalSettings" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs{SettingsPayload}"/> instance containing the event data.</param>
        [StreamDeckEvent("didReceiveGlobalSettings")]
        protected virtual Task OnDidReceiveGlobalSettings(StreamDeckEventArgs<SettingsPayload> args)
        {
            this.DidReceiveGlobalSettings?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Raises the <see cref="SystemDidWakeUp" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs"/> instance containing the event data.</param>
        protected virtual Task OnSystemDidWakeUp(StreamDeckEventArgs args)
        {
            this.SystemDidWakeUp?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the <see cref="IWebSocket.MessageReceived"/> event; triggering any associated events.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs"/> instance containing the event data.</param>
        private void WebSocket_MessageReceived(object sender, WebSocketMessageEventArgs e)
        {
            try
            {
                this.EventRouter.RouteAsync(this, e)
                    .ConfigureAwait(false);
            }
            catch (ActionInvokeException ex)
            {
                // attempt to invoke the error with a context
                this.Error?.Invoke(this, new StreamDeckClientErrorEventArgs(ex.InnerException, e.Message, ex.Context));
            }
            catch (Exception ex)
            {
                // otherwise simply invoke the error
                this.Error?.Invoke(this, new StreamDeckClientErrorEventArgs(ex, e?.Message));
            }
        }
    }
}
