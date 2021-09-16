namespace DrillDown.Actions
{
    using System.Linq;
    using System.Threading.Tasks;
    using SharpDeck;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides action handling for the "Show Numbers" action.
    /// </summary>
    [StreamDeckAction("com.geekyeggo.drilldown.shownumbers")]
    public class ShowNumbersAction : StreamDeckAction
    {
        /// <summary>
        /// Occurs when the button is pressed, typically on <see cref="IStreamDeckConnection.KeyUp"/>. When a <see cref="StreamDeckAction.LongKeyPressInterval"/> is defined, this will occur on <see cref="IStreamDeckConnection.KeyUp"/> when the interval has not elapsed in relation to <see cref="IStreamDeckConnection.KeyDown"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{TPayload}" /> instance containing the event data.</param>
        protected override async Task OnKeyPress(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyPress(args);

            var numbers = Enumerable.Range(1, 119);
            var result = await this.ShowDrillDownAsync<SelectNumberController, int>(numbers);
            await this.SetTitleAsync(result.IsSuccess ? result.Item.ToString() : string.Empty);
        }
    }
}
