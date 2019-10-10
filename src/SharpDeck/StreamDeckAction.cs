namespace SharpDeck
{
    using Enums;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Events.Received;
    using SharpDeck.PropertyInspectors;
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base implementation of an action that can be registered on a <see cref="StreamDeckClient"/>.
    /// </summary>
    public class StreamDeckAction : StreamDeckActionEventReceiver
    {
        /// <summary>
        /// Gets the property inspector method collection caches.
        /// </summary>
        private static ConcurrentDictionary<Type, PropertyInspectorMethodCollection> PropertyInspectorMethodCollections { get; } = new ConcurrentDictionary<Type, PropertyInspectorMethodCollection>();

        /// <summary>
        /// Gets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        public string ActionUUID { get; private set; }

        /// <summary>
        /// Gets an opaque value identifying the instance of the action. You will need to pass this opaque value to several APIs like the `setTitle` API.
        /// </summary>
        public string Context { get; private set; }

        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Device { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable property inspector methods.
        /// </summary>
        protected bool EnablePropertyInspectorMethods { get; set; } = true;

        /// <summary>
        /// Gets the Elgato Stream Deck client.
        /// </summary>
        protected IStreamDeckSender StreamDeck { get; private set; }

        /// <summary>
        /// Gets this action's instances settings asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <returns>The task containing the settings.</returns>
        public Task<T> GetSettingsAsync<T>()
            where T : class
        {
            var taskSource = new TaskCompletionSource<T>();

            // declare the local function handler that sets the task result
            void handler(object sender, ActionEventArgs<ActionPayload> e)
            {
                this.DidReceiveSettings -= handler;
                taskSource.TrySetResult(e.Payload.GetSettings<T>());
            }

            // listen for receiving events, and trigger a request
            this.DidReceiveSettings += handler;
            this.StreamDeck.GetSettingsAsync(this.Context);

            return taskSource.Task;
        }

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        public Task SendToPropertyInspectorAsync(object payload)
            => this.StreamDeck.SendToPropertyInspectorAsync(this.Context, this.ActionUUID, payload);

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="base64Image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetImageAsync(string base64Image, TargetType target = TargetType.Both)
            => this.StreamDeck.SetImageAsync(this.Context, base64Image, target);

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetTitleAsync(string title = "", TargetType target = TargetType.Both)
            => this.StreamDeck.SetTitleAsync(this.Context, title, target);

        /// <summary>
        /// Save persistent data for the actions instance.
        /// </summary>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        public Task SetSettingsAsync(object settings)
            => this.StreamDeck.SetSettingsAsync(this.Context, settings);

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        public Task SetStateAsync(int state = 0)
            => this.StreamDeck.SetStateAsync(this.Context, state);

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        public Task ShowAlertAsync()
            => this.StreamDeck.ShowAlertAsync(this.Context);

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        public Task ShowOkAsync()
            => this.StreamDeck.ShowOkAsync(this.Context);

        /// <summary>
        /// Sets the context and initializes the action.
        /// </summary>
        /// <param name="args">The arguments containing the context.</param>
        /// <param name="streamDeck">The Stream Deck client.</param>
        /// <returns>The task of setting the context and initialization.</returns>
        internal virtual void Initialize(ActionEventArgs<AppearancePayload> args, IStreamDeckSender streamDeck)
        {
            this.ActionUUID = args.Action;
            this.Context = args.Context;
            this.Device = args.Device;
            this.StreamDeck = streamDeck;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.StreamDeck = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Occurs when the property inspector sends a message to the plugin.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{JObject}" /> instance containing the event data.</param>
        protected internal override Task OnSendToPlugin(ActionEventArgs<JObject> args)
        {
            base.OnSendToPlugin(args);

            if (this.EnablePropertyInspectorMethods)
            {
                var factory = PropertyInspectorMethodCollections.GetOrAdd(this.GetType(), t => new PropertyInspectorMethodCollection(t));
                return factory.InvokeAsync(this, args);
            }

            return Task.CompletedTask;
        }
    }
}
