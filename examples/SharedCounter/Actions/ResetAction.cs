namespace SharedCounter.Actions
{
    using System.Threading.Tasks;
    using SharpDeck;
    using SharpDeck.Events.Received;

    /// <summary>
    /// The reset count action.
    /// </summary>
    [StreamDeckAction("com.geekyeggo.sharedcounter.reset")]
    public class ResetAction : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResetAction"/> class.
        /// </summary>
        /// <param name="counter">The counter.</param>
        public ResetAction(Counter counter)
            => this.Counter = counter;

        /// <summary>
        /// Gets the counter.
        /// </summary>
        private Counter Counter { get; }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            await this.Counter.ResetAsync();
            await base.OnKeyDown(args);
        }
    }
}
