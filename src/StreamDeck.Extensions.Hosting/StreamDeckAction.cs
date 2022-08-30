namespace StreamDeck
{
    using System.Text.Json.Nodes;
    using StreamDeck.Events;

    /// <summary>
    /// Provides information about an action, and methods invoked throughout its lifetime.
    /// </summary>
    public class StreamDeckAction
    {
        /// <summary>
        /// Gets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        public string? ActionUUID { get; internal set; }

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        public IStreamDeckConnection? Connection { get; internal set; }

        /// <summary>
        /// Gets the value used to identify this instance's action. You will need to pass this opaque value to several APIs like the setTitle API.
        /// </summary>
        public string? Context { get; internal set; }

        /// <summary>
        /// Occurs when <see cref="GetSettingsAsync(string, CancellationToken)"/> has been called to retrieve the persistent data stored for the action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceivesettings"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        public virtual void OnDidReceiveSettings(ActionEventArgs<ActionPayload> args) { }

        /// <summary>
        /// Occurs when the user presses a key.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#keydown"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        public virtual void OnKeyDown(ActionEventArgs<KeyPayload> args) { }

        /// <summary>
        /// Occurs when the user releases a key.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#keyup"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        public virtual void OnKeyUp(ActionEventArgs<KeyPayload> args) { }

        /// <summary>
        /// Occurs when the property inspector sends a message to the plugin.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#sendtoplugin"/>.
        /// </summary>
        /// <param name="args">The <see cref="PartialActionEventArgs{JsonObject}" /> instance containing the event data.</param>
        public virtual void OnSendToPlugin(PartialActionEventArgs<JsonObject> args) { }

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#titleparametersdidchange"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{TitlePayload}" /> instance containing the event data.</param>
        public virtual void OnTitleParametersDidChange(ActionEventArgs<TitlePayload> args) { }

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#willappear"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        public virtual void OnWillAppear(ActionEventArgs<ActionPayload> args) { }

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#willdisappear"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        public virtual void OnWillDisappear(ActionEventArgs<ActionPayload> args) { }
    }
}
