namespace SharedCounter.Actions
{
    using System.Threading.Tasks;
    using SharpDeck;
    using SharpDeck.Events.Received;
    using SharpDeck.Manifest;

    /// <summary>
    /// The reset count action.
    /// </summary>
    [StreamDeckAction("Reset Count", "com.geekyeggo.sharedcounter.reset")]
    public class ResetAction : StreamDeckAction
    {
        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.KeyDown" /> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{T}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        protected override Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            Count.Instance.Reset();
            return base.OnKeyDown(args);
        }
    }
}
