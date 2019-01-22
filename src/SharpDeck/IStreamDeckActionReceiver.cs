namespace SharpDeck
{
    using Events;
    using Models;
    using System;

    /// <summary>
    /// Provides events that are received for an action from an Elgato Stream Deck.
    /// </summary>
    public interface IStreamDeckActionReceiver
    {
        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        event EventHandler<ActionEventArgs<KeyPayload>> KeyDown;

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        event EventHandler<ActionEventArgs<KeyPayload>> KeyUp;

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        event EventHandler<ActionEventArgs<TitlePayload>> TitleParametersDidChange;

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        event EventHandler<ActionEventArgs<ActionPayload>> WillAppear;

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        event EventHandler<ActionEventArgs<ActionPayload>> WillDisappear;
    }
}
