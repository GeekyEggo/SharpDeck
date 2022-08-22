namespace SharpDeck.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for <see cref="SemaphoreSlim"/>.
    /// </summary>
    internal static class SemaphoreSlimExtensions
    {
        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim"/>, while observing the <paramref name="cancellationToken"/>, asynchronously.
        /// </summary>
        /// <param name="semaphoreSlim">The semaphore slim.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The disposable lock; upon disposing the lock is released.</returns>
        public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken = default)
        {
            await semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            return new SemaphoreSlimDisposableLock(semaphoreSlim);
        }

        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim"/>, while observing the <paramref name="cancellationToken"/>.
        /// </summary>
        /// <param name="semaphoreSlim">The semaphore slim.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The disposable lock; upon disposing the lock is released.</returns>
        public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken = default)
        {
            semaphoreSlim.Wait(cancellationToken);
            return new SemaphoreSlimDisposableLock(semaphoreSlim);
        }

        /// <summary>
        /// Provides a disposable wrapper for a <see cref="SemaphoreSlim"/> that releases upon disposing.
        /// </summary>
        private sealed class SemaphoreSlimDisposableLock : IDisposable
        {
            /// <summary>
            /// The semaphore slim.
            /// </summary>
            private SemaphoreSlim _semaphoreSlim;

            /// <summary>
            /// Initializes a new instance of the <see cref="SemaphoreSlimDisposableLock"/> class.
            /// </summary>
            /// <param name="semaphoreSlim">The semaphore slim.</param>
            public SemaphoreSlimDisposableLock(SemaphoreSlim semaphoreSlim)
                => this._semaphoreSlim = semaphoreSlim;

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                var semaphoreSlim = Interlocked.CompareExchange(ref this._semaphoreSlim, null, this._semaphoreSlim);
                semaphoreSlim?.Release();

                GC.SuppressFinalize(this);
            }
        }
    }
}
