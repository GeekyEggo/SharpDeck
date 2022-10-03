namespace StreamDeck.Extensions.Routing
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a <see cref="IDispatcher"/> that dispatches actions, non-blocking asynchronously.
    /// </summary>
    internal sealed class AsyncDispatcher : IDispatcher, IAsyncDisposable
    {
        /// <summary>
        /// The active task count.
        /// </summary>
        private int _activeTaskCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDispatcher"/> class.
        /// </summary>
        /// <param name="connection">The connection to the Stream Deck.</param>
        public AsyncDispatcher(IStreamDeckConnection connection)
            => this.Connection = connection;

        /// <summary>
        /// Gets the connection to the Stream Deck.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        private bool IsDisposed { get; set; }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            this.IsDisposed = true;
            while (Interlocked.CompareExchange(ref this._activeTaskCount, 0, 0) != 0)
            {
                await Task.Delay(100);
            }
        }

        /// <inheritdoc/>
        public void Invoke(Func<Task> action, string context = "")
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(AsyncDispatcher));
            }

            Interlocked.Increment(ref this._activeTaskCount);
            Task.Factory.StartNew(async (state) =>
            {
                var ctx = (AsyncExecutionContext)state!;

                try
                {
                    await ctx.Invoke().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await this.Connection.LogMessageAsync(ex.Message);
                    if (!string.IsNullOrWhiteSpace(ctx.Context))
                    {
                        await this.Connection.ShowAlertAsync(ctx.Context);
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref this._activeTaskCount);
                }
            },
            new AsyncExecutionContext(action, context),
            TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// Provides information about an asynchronous event execution.
        /// </summary>
        private struct AsyncExecutionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AsyncExecutionContext"/> struct.
            /// </summary>
            /// <param name="action">The action to invoke.</param>
            /// <param name="context">The context of the <paramref name="action"/>.</param>
            public AsyncExecutionContext(Func<Task> action, string context)
            {
                this.Context = context;
                this.Invoke = action;
            }

            /// <summary>
            /// Gets the context.
            /// </summary>
            public string Context { get; }

            /// <summary>
            /// Gets the action to invoke.
            /// </summary>
            public Func<Task> Invoke { get; }
        }
    }
}
