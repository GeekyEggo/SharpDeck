namespace SharedCounter.Actions
{
    using System;
    using System.Threading.Tasks;
    using SharpDeck;
    using SharpDeck.Events.Received;

    /// <summary>
    /// The shared counter action; displays the count, and increments the count each press.
    /// </summary>
    [StreamDeckAction("com.geekyeggo.sharedcounter.counter")]
    public class CounterAction : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CounterAction"/> class.
        /// </summary>
        /// <param name="counter">The counter.</param>
        public CounterAction(Counter counter)
            => this.Counter = counter;

        /// <summary>
        /// Gets the counter.
        /// </summary>
        private Counter Counter { get; }

        /// <inheritdoc/>
        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);

            this.Counter.Changed += this.Count_CountChanged;
            await this.SetTitleAsync((await this.Counter.GetValueAsync()).ToString());
        }

        /// <inheritdoc/>
        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            this.Counter.Changed -= this.Count_CountChanged;
        }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            await this.Counter.IncrementAsync();
            await base.OnKeyDown(args);
        }

        /// <summary>
        /// Handles the <see cref="Count.CountChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Count_CountChanged(object sender, int e)
            => this.SetTitleAsync(e.ToString());
    }
}
