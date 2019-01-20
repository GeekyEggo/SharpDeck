using SharpDeck.Events;
using SharpDeck.Models;

namespace SharpDeck.Plugin
{
    public class CounterAction : StreamDeckAction
    {
        private int Count { get; set; }

        protected override void OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            this.Count = this.Count + 1;
            this.SetTitleAsync(this.Count.ToString());
        }

        protected override void OnWillAppear(ActionEventArgs<ActionPayload> args)
            => this.SetTitleAsync("0");
    }
}
