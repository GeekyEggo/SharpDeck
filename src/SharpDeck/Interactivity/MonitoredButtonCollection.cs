namespace SharpDeck.Interactivity
{
    using System;
    using System.Threading.Tasks;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides a monitored layout map of buttons for a <see cref="IdentifiableDeviceInfo" />.
    /// </summary>
    public sealed class MonitoredButtonCollection : IDisposable
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoredButtonCollection" /> class.
        /// </summary>
        /// <param name="connection">The connection to the Stream Deck.</param>
        /// <param name="device">The device to monitor.</param>
        public MonitoredButtonCollection(IStreamDeckConnection connection, IDevice device)
        {
            this.Connection = connection;
            this.Device = device;

            this.Buttons = new IButton[device.Size.Columns * device.Size.Rows];

            this.Connection.WillAppear += Connection_WillAppear;
            this.Connection.WillDisappear += Connection_WillDisappear;
        }

        /// <summary>
        /// Gets the button for the specified coordinates; otherwise <c>null</c>.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns>The button at the specified <paramref name="coordinates"/>.</returns>
        public IButton this[Coordinates coordinates]
        {
            get => this.TryGetIndex(this.Device.Id, coordinates, out var index) ? this[index] : default;
        }

        /// <summary>
        /// Gets the button for the specified index; otherwise <c>null</c>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The button at the specified <paramref name="index"/>.</returns>
        public IButton this[int index]
        {
            get => index >= 0 && index < this.Length ? this.Buttons[index] : null;
            private set => this.Buttons[index] = value;
        }

        /// <summary>
        /// Gets the total number of buttons available on the device.
        /// </summary>
        public int Length => this.Buttons.Length;

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
        private IButton[] Buttons { get; }

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
                        this.TrackedCount = Math.Min(this.TrackedCount + 1, this.Length);
                    }

                    this[index] = new StreamDeckButton(this.Connection, e.Context);
                    if (this.TrackedCount == this.Length)
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
                    this.Buttons[index] = default;
                    this.TrackedCount = Math.Max(this.TrackedCount - 1, 0);
                }
            }
        }
    }
}
