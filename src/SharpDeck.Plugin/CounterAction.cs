namespace SharpDeck.Plugin
{
    using Events;
    using System.Threading.Tasks;

    /// <summary>
    /// A simple counter action.
    /// </summary>
    public class CounterAction : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CounterAction"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        public CounterAction(int count)
        {
            this.Count = count;
        }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        private int Count { get; set; }

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        /// <param name="args">The <see cref="T:SharpDeck.Events.ActionEventArgs`1" /> instance containing the event data.</param>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            this.Count = this.Count + 1;
            if (this.Count == 3)
            {
                await this.ShowOkAsync();
                await Task.Delay(1000);
            }

            await this.SetTitleAsync(this.Count.ToString());
        }

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        /// <param name="args">The <see cref="T:SharpDeck.Events.ActionEventArgs`1" /> instance containing the event data.</param>
        protected override Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
            => this.SetTitleAsync(this.Count.ToString());
    }
}
