namespace SharpDeck.Events
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides a handler for an event received from an Elgato Stream Deck for an instance of an action.
    /// </summary>
    public class StreamDeckActionEventPropagator : IStreamDeckActionEventPropagator, IDisposable
    {
        /// <summary>
        /// Occurs when <see cref="IStreamDeckSender.GetSettingsAsync(string)"/> has been called to retrieve the persistent data stored for the action.
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Raises the <see cref="DidReceiveSettings"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
            => this.InvokeAsync(this.DidReceiveSettings, args);

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnKeyDown(ActionEventArgs<KeyPayload> args)
            => this.InvokeAsync(this.KeyDown, args);

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnKeyUp(ActionEventArgs<KeyPayload> args)
            => this.InvokeAsync(this.KeyUp, args);

        /// <summary>
        /// Raises the <see cref="PropertyInspectorDidAppear"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs" /> instance containing the event data.</param>
        protected internal virtual Task OnPropertyInspectorDidAppear(ActionEventArgs args)
            => this.InvokeAsync(this.PropertyInspectorDidAppear, args);

        /// <summary>
        /// Raises the <see cref="PropertyInspectorDidDisappear"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs" /> instance containing the event data.</param>
        protected internal virtual Task OnPropertyInspectorDidDisappear(ActionEventArgs args)
            => this.InvokeAsync(this.PropertyInspectorDidDisappear, args);

        /// <summary>
        /// Occurs when the property inspector sends a message to the plugin.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{JObject}"/> instance containing the event data.</param>
        protected internal virtual Task OnSendToPlugin(ActionEventArgs<JObject> args)
            => this.InvokeAsync(this.SendToPlugin, args);

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{TitlePayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnTitleParametersDidChange(ActionEventArgs<TitlePayload> args)
            => this.InvokeAsync(this.TitleParametersDidChange, args);

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
            => this.InvokeAsync(this.WillAppear, args);

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        protected internal virtual Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
            => this.InvokeAsync(this.WillDisappear, args);

        /// <summary>
        /// Invokes the specified event, and returns a completed tassk.
        /// </summary>
        /// <typeparam name="T">The event argument type</typeparam>
        /// <param name="event">The event to invoke.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The task of invoking the event.</returns>
        protected Task InvokeAsync<T>(EventHandler<T> @event, T args)
        {
            @event?.Invoke(this, args);
            return Task.CompletedTask;
        }
    }
}
