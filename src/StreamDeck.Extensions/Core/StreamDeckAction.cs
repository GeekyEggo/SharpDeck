namespace StreamDeck
{
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization.Metadata;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides information about an action, and methods invoked throughout its lifetime.
    /// </summary>
    public class StreamDeckAction : IDisposable
    {
        /// <summary>
        /// The context
        /// </summary>
        private string? _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckAction"/> class.
        /// </summary>
        /// <param name="context">The action's initialization context.</param>
        public StreamDeckAction(ActionInitializationContext context)
        {
            this.ActionUUID = context.ActionInfo.Action;
            this.Connection = context.Connection;
            this._context = context.ActionInfo.Context;
            this.Logger = context.Logger;
        }

        /// <summary>
        /// Gets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        public string ActionUUID { get; }

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        public IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the value used to identify this instance's action. You will need to pass this opaque value to several APIs like the setTitle API.
        /// </summary>
        public string Context => this._context ?? throw new ObjectDisposedException(nameof(StreamDeckAction), "The action has been disposed, and no longer has a context.");

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected internal ILogger? Logger { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Requests the persistent data stored for the specified context's action instance.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#getsettings"/>.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of sending the message; this result does not contain the settings.</returns>
        public Task GetSettingsAsync(CancellationToken cancellationToken = default)
            => this.Connection.GetSettingsAsync(this.Context, cancellationToken);

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#sendtopropertyinspector" />.
        /// </summary>
        /// <typeparam name="TPayload">The type of the payload.</typeparam>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        /// <param name="jsonTypeInfo">The optional JSON type information used when serializing the <paramref name="payload" />.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of sending payload to the property inspector.</returns>
        public Task SendToPropertyInspectorAsync<TPayload>(TPayload payload, JsonTypeInfo<TPayload>? jsonTypeInfo = null, CancellationToken cancellationToken = default)
            => this.Connection.SendToPropertyInspectorAsync(this.Context, this.ActionUUID, payload, jsonTypeInfo, cancellationToken);

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action; starting with Stream Deck 4.5.1, this API accepts svg images.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setimage"/>.
        /// </summary>
        /// <param name="image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). svg is also supported. If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the image is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the image.</returns>
        public Task SetImageAsync(string image = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
            => this.Connection.SetImageAsync(this.Context, image, target, state, cancellationToken);

        /// <summary>
        /// Save persistent data for the actions instance.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setsettings" />.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        /// <param name="jsonTypeInfo">The optional JSON type information used when serializing the <paramref name="settings" />.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the settings.</returns>
        public Task SetSettingsAsync<TSettings>(TSettings settings, JsonTypeInfo<TSettings>? jsonTypeInfo = null, CancellationToken cancellationToken = default)
            where TSettings : class
            => this.Connection.SetSettingsAsync(this.Context, settings, jsonTypeInfo, cancellationToken);

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        ///	<see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setstate"/>.
        /// </summary>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the state.</returns>
        public Task SetStateAsync(int state, CancellationToken cancellationToken = default)
            => this.Connection.SetStateAsync(this.Context, state, cancellationToken);

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#settitle"/>.
        /// </summary>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the title.</returns>
        public Task SetTitleAsync(string title = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
            => this.Connection.SetTitleAsync(this.Context, title, target, state, cancellationToken);

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#showalert"/>.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the alert.</returns>
        public Task ShowAlertAsync(CancellationToken cancellationToken = default)
            => this.Connection.ShowAlertAsync(this.Context, cancellationToken);

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#showok"/>.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the OK.</returns>
        public Task ShowOkAsync(CancellationToken cancellationToken = default)
            => this.Connection.ShowOkAsync(this.Context, cancellationToken);

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.GetSettingsAsync(string, CancellationToken)"/> has been called to retrieve the persistent data stored for the action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceivesettings"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when the user presses a key.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#keydown"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnKeyDown(ActionEventArgs<KeyPayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when the user releases a key.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#keyup"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnKeyUp(ActionEventArgs<KeyPayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when the Property Inspector appears.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#propertyinspectordidappear" />.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs"/> instance containing the event data.</param>
        protected internal virtual Task OnPropertyInspectorDidAppear(ActionEventArgs args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when the Property Inspector disappears
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#propertyinspectordiddisappear" />.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs"/> instance containing the event data.</param>
        protected internal virtual Task OnPropertyInspectorDidDisappear(ActionEventArgs args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when the property inspector sends a message to the plugin.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#sendtoplugin"/>.
        /// </summary>
        /// <param name="args">The <see cref="PartialActionEventArgs{JsonObject}" /> instance containing the event data.</param>
        protected internal virtual Task OnSendToPlugin(PartialActionEventArgs<JsonObject> args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#titleparametersdidchange"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{TitlePayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnTitleParametersDidChange(ActionEventArgs<TitlePayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#willappear"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnWillAppear(ActionEventArgs<ActionPayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#willdisappear"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnWillDisappear(ActionEventArgs<ActionPayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="dispoing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool dispoing)
        {
            if (!this.IsDisposed)
            {
                this._context = null;
                this.IsDisposed = true;
            }
        }
    }
}
