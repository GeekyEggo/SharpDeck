namespace SharedCounter
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides methods for monitoring and persisting the shared count.
    /// </summary>
    public class Counter : IHostedService
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="Counter"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Counter(IStreamDeckConnection connection)
            => this.Connection = connection;

        /// <summary>
        /// Occurs when the value has changed.
        /// </summary>
        public event EventHandler<int> Changed;

        /// <summary>
        /// Gets the connection to the Stream Deck.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        private int Value { get; set; }

        /// <summary>
        /// Gets the task responsible for indicating this instance is ready.
        /// </summary>
        private TaskCompletionSource<object> Started { get; } = new TaskCompletionSource<object>();

        /// <summary>
        /// Gets the value asynchronously.
        /// </summary>
        /// <returns>The value of the counter.</returns>
        public async ValueTask<int> GetValueAsync()
        {
            await this.Started.Task;
            return this.Value;
        }

        /// <summary>
        /// Increments the count asynchronously.
        /// </summary>
        public ValueTask IncrementAsync()
            => this.UpdateAsync(() => this.Value++);

        /// <summary>
        /// Resets the count asynchronously.
        /// </summary>
        public ValueTask ResetAsync()
            => this.UpdateAsync(() => this.Value = 0);

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var settings = await this.Connection.GetGlobalSettingsAsync<GlobalSettings>();

            this.Value = settings.Count;
            this.Changed?.Invoke(this, this.Value);

            this.Started.TrySetResult(true);
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        /// <summary>
        /// Updates the value asynchronous.
        /// </summary>
        /// <param name="action">The action to apply.</param>
        private async ValueTask UpdateAsync(Action action)
        {
            await this.Started.Task;

            try
            {
                await this._syncRoot.WaitAsync();

                action();
                await this.Connection.SetGlobalSettingsAsync(new GlobalSettings
                {
                    Count = this.Value
                });
            }
            finally
            {
                this._syncRoot.Release();
            }

            this.Changed?.Invoke(this, this.Value);
        }
    }
}
