namespace SharpDeck
{
    using SharpDeck.Events.Received;
    using System;

    /// <summary>
    /// Provides events that are received for an action from an Elgato Stream Deck.
    /// </summary>
    public interface IStreamDeckActionReceiver
    {
        /// <summary>
        /// Occurs when <see cref="IStreamDeckSender.GetSettingsAsync(string)"/> has been called to retrieve the persistent data stored for the action.
        /// </summary>
        event EventHandler<ActionEventArgs<ActionPayload>> DidReceiveSettings;

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        event EventHandler<ActionEventArgs<KeyPayload>> KeyDown;

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        event EventHandler<ActionEventArgs<KeyPayload>> KeyUp;

        /// <summary>
        /// Occurs when the Property Inspector appears.
        /// </summary>
        event EventHandler<ActionEventArgs> PropertyInspectorDidAppear;

        /// <summary>
        /// Occurs when the Property Inspector disappears
        /// </summary>
        event EventHandler<ActionEventArgs> PropertyInspectorDidDisappear;

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        event EventHandler<ActionEventArgs<TitlePayload>> TitleParametersDidChange;

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        event EventHandler<ActionEventArgs<AppearancePayload>> WillAppear;

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        event EventHandler<ActionEventArgs<AppearancePayload>> WillDisappear;
    }
}
