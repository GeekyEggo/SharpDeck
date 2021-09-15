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
            this.Context.Connection.KeyDown += Connection_KeyDown;
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
        /// Gets or sets the index of the current page.
        /// </summary>
        private int PageIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the page items.
        /// </summary>
        private TItem[] PageItems { get; set; }

        /// <summary>
        /// Gets or sets the available pager buttons.
        /// </summary>
        private NavigationButtons PagerButtons { get; set; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                this._syncRoot.Wait();
                this.Dispose(true);
            }
            finally
            {
                this._syncRoot.Release();
            }

            GC.SuppressFinalize(this);
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
            await this.ShowCurrentPageAsync();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                this.Buttons.Dispose();
                this.Context.Connection.KeyDown -= this.Connection_KeyDown;

                this.PageChangingCancellationTokenSource?.Cancel();
                this.PageChangingCancellationTokenSource?.Dispose();

                _ = this.Context.Connection.SwitchToProfileAsync(this.Context.PluginUUID, this.Context.Device.Id);

                this.DataSource = null;
                this.PageItems = null;

                this.IsDisposed = true;
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the Connection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs{KeyPayload}"/> instance containing the event data.</param>
        private void Connection_KeyDown(object sender, ActionEventArgs<KeyPayload> e)
        {
            try
            {
                this._syncRoot.Wait();

                if (this.IsDisposed)
                {
                    return;
                }

                switch (e.Payload.Coordinates)
                {
                    // Close button.
                    case Coordinates c when c.Column == 0 && c.Row == 0:
                        this.Dispose(false);
                        break;

                    // Last button.
                    case Coordinates c when
                        c.Column == this.Context.Device.Size.Columns - 1 && c.Row == this.Context.Device.Size.Rows - 1 && this.PagerButtons != NavigationButtons.None:
                        {
                            if (this.PagerButtons.HasFlag(NavigationButtons.Next))
                            {
                                this.PageIndex++;
                            }
                            else
                            {
                                this.PageIndex = Math.Max(this.PageIndex - 1, 0);
                            }

                            _ = this.ShowCurrentPageAsync();
                        }
                        break;

                    // One from last button
                    case Coordinates c when
                        c.Column == this.Context.Device.Size.Columns - 2 && c.Row == this.Context.Device.Size.Rows - 1 && this.PagerButtons.HasFlag(NavigationButtons.Previous) && this.PagerButtons.HasFlag(NavigationButtons.Next):
                        {
                            this.PageIndex = Math.Max(this.PageIndex - 1, 0);
                            _ = this.ShowCurrentPageAsync();
                        }
                        break;

                    // All other buttons
                    default:
                        if (this.Buttons.TryGetIndex(e.Device, e.Payload.Coordinates, out var index))
                        {
                            // We subtract one to account for the "close" button.
                            index--;
                            if (index < this.PageItems.Length)
                            {
                                var item = this.PageItems[index];
                                _ = Task.Run(() => this.Manager.OnSelectedAsync(this.Context, item, CancellationToken.None));
                            }
                        }
                        break;
                }
            }
            finally
            {
                this._syncRoot.Release();
            }
        }

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillDisappear"/> event for an action; this is a safety precaution in the event an action switch profiles away from the drill-down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private void Connection_WillDisappear(object sender, ActionEventArgs<AppearancePayload> e)
            => this.Dispose(false);

        private async Task ShowCurrentPageAsync()
        {
            try
            {
                await this._syncRoot.WaitAsync();

                if (this.IsDisposed)
                {
                    return;
                }

                this.PageChangingCancellationTokenSource?.Cancel();
                this.PageChangingCancellationTokenSource = new CancellationTokenSource();

                if (this.PageIndex == 0)
                {
                    var items = this.DataSource
                        .Skip(this.PageIndex)
                        .Take(this.Buttons.Count);

                    // When there is a full page of buttons, we need to display paging; this is to allow us to show the top left button as an "close" button.
                    if (items.Count() == this.Buttons.Count)
                    {
                        await this.ShowAsync(items.Take(this.Buttons.Count - 2).ToArray(), NavigationButtons.Next, this.PageChangingCancellationTokenSource.Token);
                    }
                    else
                    {
                        await this.ShowAsync(items.ToArray(), NavigationButtons.None, this.PageChangingCancellationTokenSource.Token);
                    }
                }
                else
                {
                    var offset = (this.Buttons.Count - 2) + ((this.PageIndex - 1) * (this.Buttons.Count - 3));
                    var items = this.DataSource
                        .Skip(offset)
                        .Take(this.Buttons.Count - 1)
                        .ToArray(); // We know we must display a back button.

                    if (items.Length >= this.Buttons.Count - 1)
                    {
                        await this.ShowAsync(items.Take(this.Buttons.Count - 3).ToArray(), NavigationButtons.Previous | NavigationButtons.Next, this.PageChangingCancellationTokenSource.Token);
                    }
                    else
                    {
                        await this.ShowAsync(items.ToArray(), NavigationButtons.Previous, this.PageChangingCancellationTokenSource.Token);
                    }
                }
            }
            finally
            {
                this._syncRoot.Release();
            }
        }

        private async Task ShowAsync(TItem[] items, NavigationButtons pagerButtons, CancellationToken cancellationToken)
        {
            const int offset = 1;
            var pagerButtonCount = pagerButtons == NavigationButtons.None
                ? this.Buttons.Count
                : pagerButtons.HasFlag(NavigationButtons.Next) && pagerButtons.HasFlag(NavigationButtons.Previous)
                    ? this.Buttons.Count - 2
                    : this.Buttons.Count - 1;

            for (var i = 0; i < pagerButtonCount; i++)
            {
                if (i < items.Length)
                {
                    this.Manager
                        .OnShowAsync(this.Context, new ButtonFeedbackProvider(this.Context.Connection, this.Buttons[i + offset].Context), items[i], cancellationToken)
                        .Forget(this.Logger);
                }
                else
                {
                    await this.Context.Connection.SetTitleAsync(this.Buttons[i + offset].Context, cancellationToken: cancellationToken);
                }
            }

            this.PagerButtons = pagerButtons;
            this.PageItems = items;

            if (this.PagerButtons.HasFlag(NavigationButtons.Previous) && this.PagerButtons.HasFlag(NavigationButtons.Next))
            {
                await this.Context.Connection.SetTitleAsync(this.Buttons[this.Buttons.Count - 2].Context, "<", cancellationToken: cancellationToken);
                await this.Context.Connection.SetTitleAsync(this.Buttons[this.Buttons.Count - 1].Context, ">", cancellationToken: cancellationToken);
            }
            else if (this.PagerButtons.HasFlag(NavigationButtons.Previous))
            {
                await this.Context.Connection.SetTitleAsync(this.Buttons[this.Buttons.Count - 1].Context, "<", cancellationToken: cancellationToken);
            }
            else if (this.PagerButtons.HasFlag(NavigationButtons.Next))
            {
                await this.Context.Connection.SetTitleAsync(this.Buttons[this.Buttons.Count - 1].Context, ">", cancellationToken: cancellationToken);
            }
        }
    }
}
