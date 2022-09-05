namespace StreamDeck.Routing
{
    using System;
    using StreamDeck.Events;

    /// <summary>
    /// Provides a <see cref="IEventDispatcher"/> that dispatches events non-blocking asynchronously.
    /// </summary>
    internal class AsyncEventDispatcher : IEventDispatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncEventDispatcher"/> class.
        /// </summary>
        /// <param name="connection">The connection to the Stream Deck.</param>
        public AsyncEventDispatcher(IStreamDeckConnection connection)
            => this.Connection = connection;

        /// <summary>
        /// Gets the connection to the Stream Deck.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <inheritdoc/>
        public void Invoke<TArgs>(Func<TArgs, Task> @event, TArgs args)
            where TArgs : IActionContext
        {
            Task.Factory.StartNew(async (state) =>
            {
                var ctx = (AsyncExecutionContext<TArgs>)state!;

                try
                {
                    await ctx.Invoke(ctx.Arguments).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await this.Connection.ShowAlertAsync(ctx.Arguments.Context);
                    await this.Connection.LogMessageAsync(ex.Message);
                }
            },
            new AsyncExecutionContext<TArgs>(@event, args),
            TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// Provides information about an asynchronous event execution.
        /// </summary>
        /// <typeparam name="TArgs">The type of event arguments.</typeparam>
        private struct AsyncExecutionContext<TArgs>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AsyncExecutionContext{TArgs}"/> struct.
            /// </summary>
            /// <param name="event">The event to invoke.</param>
            /// <param name="args">The arguments to supply to the <paramref name="event"/>.</param>
            public AsyncExecutionContext(Func<TArgs, Task> @event, TArgs args)
            {
                this.Invoke = @event;
                this.Arguments = args;
            }

            /// <summary>
            /// Gets the arguments.
            /// </summary>
            public TArgs Arguments { get; }

            /// <summary>
            /// Gets the action to invoke.
            /// </summary>
            public Func<TArgs, Task> Invoke { get; }
        }
    }
}
