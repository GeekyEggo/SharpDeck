namespace SharpDeck
{
    using Newtonsoft.Json.Linq;
    using SharpDeck.Events.Received;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a handler for an event received from an Elgato Stream Deck for an instance of an action.
    /// </summary>
    public class StreamDeckActionEventReceiver : IStreamDeckActionReceiver, IDisposable
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
        protected virtual void Dispose(bool disposing) {}

        /// <summary>
        /// Raises the <see cref="DidReceiveSettings"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("didReceiveSettings")]
        protected virtual Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            this.DidReceiveSettings?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("keyDown")]
        protected virtual Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            this.KeyDown?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("keyUp")]
        protected virtual Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            this.KeyUp?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Raises the <see cref="PropertyInspectorDidAppear"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs" /> instance containing the event data.</param>
        [StreamDeckEvent("propertyInspectorDidAppear")]
        protected virtual Task OnPropertyInspectorDidAppear(ActionEventArgs args)
        {
            this.PropertyInspectorDidAppear?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Raises the <see cref="PropertyInspectorDidDisappear"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs" /> instance containing the event data.</param>
        [StreamDeckEvent("propertyInspectorDidDisappear")]
        protected virtual Task OnPropertyInspectorDidDisappear(ActionEventArgs args)
        {
            this.PropertyInspectorDidDisappear?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when the property inspector sends a message to the plugin.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{JObject}"/> instance containing the event data.</param>
        [StreamDeckEvent("sendToPlugin")]
        protected virtual Task OnSendToPlugin(ActionEventArgs<JObject> args)
        {
            this.SendToPlugin?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{TitlePayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("titleParametersDidChange")]
        protected virtual Task OnTitleParametersDidChange(ActionEventArgs<TitlePayload> args)
        {
            this.TitleParametersDidChange?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("willAppear")]
        protected virtual Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            this.WillAppear?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("willDisappear")]
        protected virtual Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            this.WillDisappear?.Invoke(this, args);
            return Task.CompletedTask;
        }
    }
}
