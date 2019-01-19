namespace SharpDeck.Actions
{
    using SharpDeck.Events;
    using SharpDeck.Models;

    public interface IStreamDeckAction
    {
        /// <summary>
        /// Initializes the action.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="streamDeck">An Elgato Stream Deck client.</param>
        void Initialize(IActionInfo info, StreamDeckClient client);

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        /// <param name="payload">The payload, containing information about the key.</param>
        [StreamDeckEvent("keyDown")]
        void OnKeyDown(KeyPayload payload);

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        /// <param name="payload">The payload, containing information about the key.</param>
        void OnKeyUp(KeyPayload payload);

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        /// <param name="payload">The payload, containing information about the title.</param>
        void OnTitleParametersDidChange(TitlePayload payload);

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        /// <param name="payload">The payload, containing information about the action.</param>
        [StreamDeckEvent("willAppear")]
        void OnWillAppear(ActionPayload payload);

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        /// <param name="payload">The payload, containing information about the action.</param>
        void OnWillDisappear(ActionPayload payload);
    }
}
