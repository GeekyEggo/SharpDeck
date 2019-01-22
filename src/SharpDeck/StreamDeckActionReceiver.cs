namespace SharpDeck
{
    using Models;
    using SharpDeck.Events;
    using System;

    /// <summary>
    /// Provides a handler for an event received from an Elgato Stream Deck for an instance of an action.
    /// </summary>
    public class StreamDeckActionReceiver : IStreamDeckActionReceiver, IDisposable
    {
        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        public event EventHandler<ActionEventArgs<KeyPayload>> KeyDown;

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        public event EventHandler<ActionEventArgs<KeyPayload>> KeyUp;

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        public event EventHandler<ActionEventArgs<TitlePayload>> TitleParametersDidChange;

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        public event EventHandler<ActionEventArgs<ActionPayload>> WillAppear;

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        public event EventHandler<ActionEventArgs<ActionPayload>> WillDisappear;

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
        /// Occurs when the user presses a key.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("keyDown")]
        protected virtual void OnKeyDown(ActionEventArgs<KeyPayload> args)
            => this.KeyDown?.Invoke(this, args);

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("keyUp")]
        protected virtual void OnKeyUp(ActionEventArgs<KeyPayload> args)
            => this.KeyUp?.Invoke(this, args);

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{TitlePayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("titleParametersDidChange")]
        protected virtual void OnTitleParametersDidChange(ActionEventArgs<TitlePayload> args)
            => this.TitleParametersDidChange?.Invoke(this, args);


        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("willAppear")]
        protected virtual void OnWillAppear(ActionEventArgs<ActionPayload> args)
            => this.WillAppear?.Invoke(this, args);

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        [StreamDeckEvent("willDisappear")]
        protected virtual void OnWillDisappear(ActionEventArgs<ActionPayload> args)
            => this.WillDisappear?.Invoke(this, args);
    }
}
