namespace SharpDeck
{
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides information and methods for interacting with a drill-down Stream Deck action; this is one typically opened after long-pressing another  Stream Deck action
    /// </summary>
    /// <typeparam name="TItem">The type of item used to identify the instance of the drill-down action.</typeparam>
    public class StreamDeckDrillDownAction<TItem> : StreamDeckButton
    {
        /// <summary>
        /// Gets the item associated with this instance of drill-down action..
        /// </summary>
        protected TItem Item { get; private set; }

        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <param name="args">The <see cref="IActionEventArgs"/> instance containing the event data.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="item">The item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task of initializing the action for the item.</returns>
        internal Task InitializeAsync(IActionEventArgs args, IStreamDeckConnection connection, TItem item, CancellationToken cancellationToken)
        {
            base.Initialize(args, connection);
            return this.InitializeAsync(item, cancellationToken);
        }

        /// <summary>
        /// Initializes this instance for the given <paramref name="item"/> asynchronously.
        /// </summary>
        /// <param name="item">The item to initialize this action for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task of initializing the action for the item.</returns>
        internal protected virtual Task InitializeAsync(TItem item, CancellationToken cancellationToken)
        {
            this.Item = item;
            return Task.CompletedTask;
        }
    }
}
