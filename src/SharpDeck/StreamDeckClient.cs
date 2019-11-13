[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SharpDeck.Tests")]

namespace SharpDeck
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Connectivity;
    using SharpDeck.DependencyInjection;
    using SharpDeck.Enums;
    using SharpDeck.Events;
    using SharpDeck.Events.Received;
    using SharpDeck.Events.Sent;
    using SharpDeck.Exceptions;

    /// <summary>
    /// Provides events and methods that allow for communication with an Elgato Stream Deck.
    /// </summary>
    public sealed class StreamDeckClient : StreamDeckEventPropagator, IStreamDeckClient
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
            this.RegistrationParameters = registrationParameters;
            this.Logger = logger;

            this.Connection = new WebSocketStreamDeckConnection();
            this.Connection.Error += (s, e) => this.OnError(e);

            this.ActionProvider = new StreamDeckActionProvider(this.Connection, this);
            this.PropagateFrom(this.Connection);
        }

        /// <summary>
        /// Occurs when the client encounters an error.
        /// </summary>
        public event StreamDeckClientEventHandler<StreamDeckConnectionErrorEventArgs> Error;

        /// <summary>
        /// Gets the event router.
        /// </summary>
        private StreamDeckActionProvider ActionProvider { get; }

        /// <summary>
        /// Gets or sets the connection to the Stream Deck.
        /// </summary>
        private WebSocketStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; }

        /// <summary>
        /// Starts the <see cref="StreamDeckClient"/>.
        /// </summary>
        /// <param name="args">The optional command line arguments supplied when running the plug-in; when null, <see cref="Environment.GetCommandLineArgs"/> is used.</param>
        /// <param name="assembly">The optional assembly containing the <see cref="StreamDeckAction"/>; when null, <see cref="Assembly.GetEntryAssembly"/> is used.</param>
        /// <param name="provider">The optional service provider to resolve new instances of the registered <see cref="StreamDeckAction"/>.</param>
        /// <param name="logger">The optional logger.</param>
        /// <param name="setup">The optional additional setup.</param>
        public static async void Run(string[] args = null, Assembly assembly = null, IServiceProvider provider = null, ILogger logger = null, Action<IStreamDeckClient> setup = null)
        {
            await RunAsync(args, assembly ?? Assembly.GetCallingAssembly(), provider, logger, setup).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts the <see cref="StreamDeckClient"/> asynchronously.
        /// </summary>
        /// <param name="args">The optional command line arguments supplied when running the plug-in; when null, <see cref="Environment.GetCommandLineArgs"/> is used.</param>
        /// <param name="assembly">The optional assembly containing the <see cref="StreamDeckAction"/>; when null, <see cref="Assembly.GetEntryAssembly"/> is used.</param>
        /// <param name="provider">The optional service provider to resolve new instances of the registered <see cref="StreamDeckAction"/>.</param>
        /// <param name="logger">The optional logger.</param>
        /// <param name="setup">The optional additional setup.</param>
        /// <returns>The task of running the client.</returns>
        public static Task RunAsync(string[] args = null, Assembly assembly = null, IServiceProvider provider = null, ILogger logger = null, Action<IStreamDeckClient> setup = null)
            => StreamDeckClientRunner.RunAsync(new StreamDeckClientRunnerInfo(assembly ?? Assembly.GetCallingAssembly(), args, provider, logger, setup));

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
            => this.ActionProvider.Register(actionUUID, _ => valueFactory());

        /// <summary>
        /// Registers a new <see cref="StreamDeckAction"/> for the specified action UUID.
        /// </summary>
        /// <typeparam name="T">The type of Stream Deck action.</typeparam>
        /// <param name="actionUUID">The action UUID.</param>
        /// <param name="valueFactory">The value factory, used to initialize a new action.</param>
        public void RegisterAction<T>(string actionUUID, Func<ActionEventArgs<AppearancePayload>, T> valueFactory)
            where T : StreamDeckAction
            => this.ActionProvider.Register(actionUUID, valueFactory);

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
        public Task StartAsync(CancellationToken cancellationToken)
            => this.Connection.ConnectAsync(this.RegistrationParameters, cancellationToken);

        /// <summary>
        /// Stops the client.
        /// </summary>
        public void Stop()
            => Task.WaitAll(this.Connection.DisconnectAsync());

        /// <summary>
        /// Requests the persistent global data stored for the plugin.
        /// </summary>
        /// <returns>The task of sending the message; this result does not contain the settings.</returns>
        public Task GetGlobalSettingsAsync()
            => this.Connection.SendAsync(new ContextMessage("getGlobalSettings", this.RegistrationParameters.PluginUUID));

        /// <summary>
        /// Requests the persistent data stored for the specified context's action instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The task of sending the message; the result does not contain the settings.</returns>
        public Task GetSettingsAsync(string context)
            => this.Connection.SendAsync(new ContextMessage("getSettings", context));

        /// <summary>
        /// Write a debug log to the logs file.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        public Task LogMessageAsync(string msg)
            => this.Connection.SendAsync(new Message<LogPayload>("logMessage", new LogPayload(msg)));

        /// <summary>
        /// Open a URL in the default browser.
        /// </summary>
        /// <param name="url">A URL to open in the default browser.</param>
        public Task OpenUrlAsync(string url)
            => this.Connection.SendAsync(new Message<UrlPayload>("openUrl", new UrlPayload(url)));

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="context">An opaque value identifying the instances action.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        public Task SendToPropertyInspectorAsync(string context, string action, object payload)
            => this.Connection.SendAsync(new ActionMessage<object>("sendToPropertyInspector", context, action, payload));

        /// <summary>
        /// Save persistent data for the plugin.
        /// </summary>
        /// <param name="settings">An object which persistently saved globally.</param>
        public Task SetGlobalSettingsAsync(object settings)
            => this.Connection.SendAsync(new ContextMessage<object>("setGlobalSettings", this.RegistrationParameters.PluginUUID, JObject.FromObject(settings, JsonSerializer.Create(Constants.DEFAULT_JSON_SETTINGS))));

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="base64Image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetImageAsync(string context, string base64Image, TargetType target = TargetType.Both)
            => this.Connection.SendAsync(new ContextMessage<SetImagePayload>("setImage", context, new SetImagePayload(base64Image, target)));

        /// <summary>
        /// Save persistent data for the action's instance.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="settings">An object which is persistently saved for the action's instance.</param>
        public Task SetSettingsAsync(string context, object settings)
            => this.Connection.SendAsync(new ContextMessage<object>("setSettings", context, JObject.FromObject(settings, JsonSerializer.Create(Constants.DEFAULT_JSON_SETTINGS))));

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        public Task SetStateAsync(string context, int state = 0)
            => this.Connection.SendAsync(new ContextMessage<SetStatePayload>("setState", context, new SetStatePayload(state)));

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action you want to modify.</param>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetTitleAsync(string context, string title = "", TargetType target = TargetType.Both)
            => this.Connection.SendAsync(new ContextMessage<SetTitlePayload>("setTitle", context, new SetTitlePayload(title, target)));

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        public Task ShowAlertAsync(string context)
            => this.Connection.SendAsync(new ContextMessage("showAlert", context));

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        public Task ShowOkAsync(string context)
            => this.Connection.SendAsync(new ContextMessage("showOk", context));

        /// <summary>
        /// Switch to one of the preconfigured read-only profiles.
        /// </summary>
        /// <param name="context">An opaque value identifying the plugin. This value should be set to the PluginUUID received during the registration procedure.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="profile">The name of the profile to switch to. The name should be identical to the name provided in the manifest.json file.</param>
        public Task SwitchToProfileAsync(string context, string device, string profile)
            => this.Connection.SendAsync(new DeviceMessage<SwitchToProfilePayload>("switchToProfile", context, device, new SwitchToProfilePayload(profile)));

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.ActionProvider?.Dispose();
            this.Connection?.Dispose();
        }

        /// <summary>
        /// Raises the <see cref="E:Error" /> event.
        /// </summary>
        /// <param name="e">The <see cref="StreamDeckConnectionErrorEventArgs"/> instance containing the event data.</param>
        internal void OnError(StreamDeckConnectionErrorEventArgs e)
        {
            this.Logger?.LogError(e.Exception, e.Message);
            this.Error?.Invoke(this, e);
        }
    }
}
