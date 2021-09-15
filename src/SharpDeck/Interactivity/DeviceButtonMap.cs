namespace SharpDeck.Interactivity
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides a monitored layout map of buttons for a <see cref="IdentifiableDeviceInfo" />.
    /// </summary>
    public sealed class DeviceButtonMap : IDisposable
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceButtonMap" /> class.
        /// </summary>
        /// <param name="connection">The connection to the Stream Deck.</param>
        /// <param name="device">The device to monitor.</param>
        public DeviceButtonMap(IStreamDeckConnection connection, IDevice device)
        {
            this.Connection = connection;
            this.Device = device;

            this.Items = new (string Context, Coordinates Coordinates)[device.Size.Columns * device.Size.Rows];

            this.Connection.WillAppear += Connection_WillAppear;
            this.Connection.WillDisappear += Connection_WillDisappear;
        }

        /// <summary>
        /// Gets the button information for the specified index.
        /// </summary>
        /// <value>The button's context and coordinates.</value>
        /// <param name="index">The index.</param>
        /// <returns>The button at the specified <paramref name="index"/>.</returns>
        public (string Context, Coordinates Coordinates) this[int index]
        {
            get => this.Items[index];
            private set => this.Items[index] = value;
        }

        /// <summary>
        /// Gets the total number of buttons available on the device.
        /// </summary>
        public int Count => this.Items.Length;

        /// <summary>
        /// Gets the connection to the Stream Deck.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the device information.
        /// </summary>
        private IDevice Device { get; }

        /// <summary>
        /// Gets the buttons.
        /// </summary>
        private (string Context, Coordinates Coordinates)[] Items { get; }

        /// <summary>
        /// Gets the task completion source that represents a full layout.
        /// </summary>
        private TaskCompletionSource<bool> FullLayoutTaskCompletionSource { get; } = new TaskCompletionSource<bool>();

        /// <summary>
        /// Gets or sets the number of buttons currently monitored by this instance.
        /// </summary>
        private int TrackedCount { get; set; } = 0;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Connection.WillAppear -= this.Connection_WillAppear;
            this.Connection.WillDisappear -= this.Connection_WillDisappear;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="index">The index of the action to change.</param>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the title.</returns>
        public async Task SetTitleAsync(int index, string title = "", CancellationToken cancellationToken = default)
        {
            if (index >= 0 && index < this.Items.Length)
            {
                await this.Connection.SetTitleAsync(this.Items[index].Context, title, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="coordinates">The coordinates of the action to change.</param>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the title.</returns>
        public async Task SetTitleAsync(Coordinates coordinates, string title = "", CancellationToken cancellationToken = default)
        {
            if (coordinates != null
                && this.TryGetIndex(this.Device.Id, coordinates, out var index))
            {
                await this.SetTitleAsync(index, title, cancellationToken);
            }
        }

        /// <summary>
        /// Waits for the map to become fully aware of the button layout asynchronously.
        /// </summary>
        /// <returns>The task of waiting for the full layout.</returns>
        public Task WaitFullLayoutAsync()
            => this.FullLayoutTaskCompletionSource.Task;

        /// <summary>
        /// Attempts to get the index of the button on the specified <paramref name="deviceUUID"/> at the specified <paramref name="coordinates"/> in relation to the <see cref="DeviceInfo.Size" />, going from top-left to bottom-right.
        /// </summary>
        /// <param name="deviceUUID">The device UUID.</param>
        /// <param name="coordinates">The coordinates of the button.</param>
        /// <param name="index">The index in relation to the buttons position on the device.</param>
        /// <returns><c>true</c> when the index was determined; otherwise <c>false</c>.</returns>
        public bool TryGetIndex(string deviceUUID, Coordinates coordinates, out int index)
        {
            if (deviceUUID != this.Device.Id)
            {
                index = -1;
                return false;
            }

            index = (coordinates.Row * this.Device.Size.Columns) + coordinates.Column;
            return true;
        }

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillAppear"/>, adding buttons to the internal collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private void Connection_WillAppear(object sender, ActionEventArgs<AppearancePayload> e)
        {
            lock (this._syncRoot)
            {
                if (this.TryGetIndex(e.Device, e.Payload.Coordinates, out var index))
                {
                    if (this[index] == default)
                    {
                        this.TrackedCount = Math.Min(this.TrackedCount + 1, this.Count);
                    }

                    this[index] = (e.Context, e.Payload.Coordinates);
                    if (this.TrackedCount == this.Count)
                    {
                        this.FullLayoutTaskCompletionSource?.TrySetResult(true);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillDisappear"/>, removing buttons to the internal collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private void Connection_WillDisappear(object sender, ActionEventArgs<AppearancePayload> e)
        {
            lock (this._syncRoot)
            {
                if (this.TryGetIndex(e.Device, e.Payload.Coordinates, out var index))
                {
                    this.Items[index] = default;
                    this.TrackedCount = Math.Max(this.TrackedCount - 1, 0);
                }
            }
        }
    }
}
