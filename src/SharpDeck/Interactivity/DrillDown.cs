namespace SharpDeck.Interactivity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;
    using SharpDeck.Extensions;

    /// <summary>
    /// Provides drill-down functioanlity for the given <typeparamref name="TItem" /> using actions of type <typeparamref name="TManager" />.
    /// </summary>
    /// <typeparam name="TManager">The type of the drill-down manager.</typeparam>
    /// <typeparam name="TItem">The type of the items the manager is capable of handling.</typeparam>
    public class DrillDown<TManager, TItem> : IDrillDown<TManager, TItem>
        where TManager : class, IDrillDownManager<TItem>
    {
        /// <summary>
        /// The offset that represents the presence of the close-button.
        /// </summary>
        private const int CLOSE_BUTTON_OFFSET = 1;

        /// <summary>
        /// The synchronization root.
        /// </summary>
        private readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="DrillDown{TManager, TItem}" /> class.
        /// </summary>
        /// <param name="context">The context that provides information about where and how the drill-down will be shown.</param>
        /// <param name="manager">The manager that provides functionality for handling a selection, or rendering items.</param>
        /// <param name="logger">The optional logger.</param>
        internal DrillDown(DrillDownContext context, TManager manager, ILogger<DrillDown<TManager, TItem>> logger = null)
        {
            this.Context = context;
            this.Logger = logger;
            this.Manager = manager;

            this.Buttons = new DeviceButtonMap(this.Context.Connection, this.Context.Device);
            this.Context.Connection.KeyUp += Connection_KeyUp;
        }

        /// <summary>
        /// Gets the button map.
        /// </summary>
        private DeviceButtonMap Buttons { get; }

        /// <summary>
        /// Get the context that provides information about where and how the drill-down will be shown.
        /// </summary>
        private DrillDownContext Context { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Gets the data source containing the items to show.
        /// </summary>
        private IEnumerable<TItem> DataSource { get; set; } = Enumerable.Empty<TItem>();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        private bool IsDisposed { get; set; } = false;

        /// <summary>
        /// Gets the action responsible for handling the display and selection of an item
        /// </summary>
        private TManager Manager { get; }

        /// <summary>
        /// Gets or sets the cancellation token source that is cancelled upon a page changing.
        /// </summary>
        private CancellationTokenSource PageChangingCancellationTokenSource { get; set; }

        /// <summary>
        /// Gets or sets the pager.
        /// </summary>
        private DevicePager<TItem> Pager { get; set; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            using (this._syncRoot.Lock())
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Shows the drill-down with the given items asynchronously.
        /// </summary>
        /// <param name="items">The items to show.</param>
        /// <returns>The task of showing the items.</returns>
        public async Task ShowAsync(IEnumerable<TItem> items)
        {
            await this.Context.Connection.SwitchToProfileAsync(this.Context.PluginUUID, this.Context.Device.Id, this.Manager.Profile);
            await this.Buttons.WaitFullLayoutAsync();

            this.Context.Connection.WillDisappear += Connection_WillDisappear;
            await this.Context.Connection.SetTitleAsync(this.Buttons[0].Context, "X");

            this.DataSource = items;
            this.Pager = new DevicePager<TItem>(this.Buttons, this.DataSource);
            this.ShowCurrentPage();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.Buttons.Dispose();
            this.Context.Connection.KeyUp -= this.Connection_KeyUp;
            this.Context.Connection.WillDisappear -= this.Connection_WillDisappear;

            this.PageChangingCancellationTokenSource?.Cancel();
            this.PageChangingCancellationTokenSource?.Dispose();

            this.Context.Connection.SwitchToProfileAsync(this.Context.PluginUUID, this.Context.Device.Id)
                .Forget(this.Logger);

            this.DataSource = null;
            this.Pager = null;

            this.IsDisposed = true;
        }

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.KeyUp"/> of the <see cref="IStreamDeckConnection"/> of the <see cref="Context"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs{KeyPayload}"/> instance containing the event data.</param>
        private void Connection_KeyUp(object sender, ActionEventArgs<KeyPayload> e)
        {
            using (this._syncRoot.Lock())
            {
                if (this.IsDisposed)
                {
                    return;
                }

                switch (e.Payload.Coordinates)
                {
                    // Close button.
                    case Coordinates c
                    when c.Column == 0 && c.Row == 0:
                        this.Dispose(false);
                        break;

                    // Next button.
                    case Coordinates c when c.Equals(this.Pager.NextButtonCoordinates):
                        this.Pager.MoveNext();
                        this.ShowCurrentPage();
                        break;

                    // Previous button.
                    case Coordinates c when c.Equals(this.Pager.PreviousButtonCoordinates):
                        this.Pager.MovePrevious();
                        this.ShowCurrentPage();
                        break;

                    // Default; this may not be an item depending on how many items the current page has.
                    default:
                        if (this.Buttons.TryGetIndex(e.Device, e.Payload.Coordinates, out var index))
                        {
                            index -= CLOSE_BUTTON_OFFSET;
                            if (index < this.Pager.Items.Length)
                            {
                                var item = this.Pager.Items[index];
                                _ = Task.Run(() => this.Manager.OnSelectedAsync(this.Context, item, CancellationToken.None));
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillDisappear"/> event for an action; this is a safety precaution in the event an action switch profiles away from the drill-down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private void Connection_WillDisappear(object sender, ActionEventArgs<AppearancePayload> e)
            => this.Dispose();

        /// <summary>
        /// Shows the current page's items contained within <see cref="DevicePager{T}.Items"/>.
        /// </summary>
        private void ShowCurrentPage()
        {
            if (this.IsDisposed)
            {
                return;
            }

            // The cancellation token allows us to cancel any initialization that is still occuring from a previous page load.
            this.PageChangingCancellationTokenSource?.Cancel();
            this.PageChangingCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = this.PageChangingCancellationTokenSource.Token;

            // Iterate over all available buttons.
            for (var i = 0; i < this.Buttons.Count - this.Pager.NavigationButtonCount; i++)
            {
                if (i < this.Pager.Items.Length)
                {
                    // The button has an item, so initiate it.
                    this.Manager
                        .OnShowAsync(this.Context, new ButtonFeedbackProvider(this.Context.Connection, this.Buttons[i + CLOSE_BUTTON_OFFSET].Context), this.Pager.Items[i], cancellationToken)
                        .Forget(this.Logger);
                }
                else
                {
                    // The button does not have an item, so reset it to an empty button.
                    this.Buttons.SetTitleAsync(i + CLOSE_BUTTON_OFFSET, cancellationToken: cancellationToken)
                        .Forget(this.Logger);
                }
            }

            // Finally, set the navigation buttons; these may be null.
            this.Buttons.SetTitleAsync(this.Pager.NextButtonCoordinates, ">", cancellationToken).Forget(this.Logger);
            this.Buttons.SetTitleAsync(this.Pager.PreviousButtonCoordinates, "<", cancellationToken).Forget(this.Logger);
        }
    }
}
