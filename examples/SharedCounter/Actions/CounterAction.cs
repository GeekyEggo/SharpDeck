namespace SharedCounter.Actions
{
    using System;
    using System.Threading.Tasks;
    using SharpDeck;
    using SharpDeck.Events.Received;
    using SharpDeck.Manifest;

    /// <summary>
    /// The shared counter action; displays the count, and increments the count each press.
    /// </summary>
    [StreamDeckAction("Counter", "com.geekyeggo.sharedcounter.counter")]
    public class CounterAction : StreamDeckAction
    {
        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.WillAppear" /> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{T}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        protected override Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            Count.Instance.CountChanged += this.Count_CountChanged;
            this.SetTitleAsync(Count.Instance.Value.ToString());

            return base.OnWillAppear(args);
        }

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.WillDisappear" /> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{T}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        protected override Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            Count.Instance.CountChanged -= this.Count_CountChanged;
            return base.OnWillDisappear(args);
        }

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.KeyDown" /> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{T}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        protected override Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            Count.Instance.Increment();
            return base.OnKeyDown(args);
        }

        /// <summary>
        /// Handles the <see cref="Count.CountChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Count_CountChanged(object sender, EventArgs e)
            => this.SetTitleAsync(Count.Instance.Value.ToString());
    }
}
