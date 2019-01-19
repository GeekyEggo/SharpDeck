using SharpDeck.Actions;
using SharpDeck.Models;

namespace SharpDeck.Plugin
{
    public class CounterAction : StreamDeckAction
    {
        private int Count { get; set; }

        public override void OnKeyDown(KeyPayload payload)
        {
            this.Count = this.Count + 1;
            this.SetTitleAsync(this.Count.ToString());
        }

        public override void OnWillAppear(ActionPayload payload)
            => this.SetTitleAsync("0");
    }
}
