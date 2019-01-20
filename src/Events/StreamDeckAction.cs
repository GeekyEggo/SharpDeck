namespace SharpDeck.Events
{
    using Enums;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base implementation of an action that can be registered on a <see cref="StreamDeckClient"/>.
    /// </summary>
    public class StreamDeckAction : ActionEventHandler, IDisposable
    {
        /// <summary>
        /// Gets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        public string ActionUUID { get; set; }

        /// <summary>
        /// Gets an opaque value identifying the instances action. You will need to pass this opaque value to several APIs like the `setTitle` API.
        /// </summary>
        public string Context { get; private set; }

        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Device { get; private set; }

        /// <summary>
        /// Gets or sets the Elgato Stream Deck client.
        /// </summary>
        private StreamDeckClient StreamDeckClient { get; set; }

        /// <summary>
        /// Initializes the action.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="streamDeck">An Elgato Stream Deck client.</param>
        public void Initialize(IActionEventInfo info, StreamDeckClient client)
        {
            this.ActionUUID = info.Action;
            this.Context = info.Context;
            this.Device = info.Device;

            this.StreamDeckClient = client;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
            => this.StreamDeckClient = null;

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetTitleAsync(string title = "", TargetType target = TargetType.Both)
            => this.StreamDeckClient.SetTitleAsync(this.Context, title, target);

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="base64Image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        public Task SetImageAsync(string base64Image, TargetType target = TargetType.Both)
            => this.StreamDeckClient.SetImageAsync(this.Context, base64Image, target);

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        public Task ShowAlertAsync()
            => this.StreamDeckClient.ShowAlertAsync(this.Context);

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        public Task ShowOkAsync()
            => this.StreamDeckClient.ShowOkAsync(this.Context);

        /// <summary>
        /// Save persistent data for the actions instance.
        /// </summary>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        public Task SetSettingsAsync(object settings)
            => this.StreamDeckClient.SetSettingsAsync(this.Context, settings);

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        public Task SetStateAsync(int state = 0)
            => this.StreamDeckClient.SetStateAsync(this.Context, state);

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="context">An opaque value identifying the instances action.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        public Task SendToPropertyInspectorAsync(string context, string action, object payload)
            => this.StreamDeckClient.SendToPropertyInspectorAsync(this.Context, this.ActionUUID, payload);
    }
}
