namespace SharpDeck.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// Utility class for temporarily switching <see cref="SynchronizationContext"/> implementations.
    /// </summary>
    /// <remarks>https://github.com/StephenClearyArchive/AsyncEx.Tasks/blob/master/src/Nito.AsyncEx.Tasks/SynchronizationContextSwitcher.cs</remarks>
    internal sealed class SynchronizationContextSwitcher : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextSwitcher"/> class, installing the new <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <param name="newContext">The new <see cref="SynchronizationContext"/>. This can be <c>null</c> to remove an existing <see cref="SynchronizationContext"/>.</param>
        public SynchronizationContextSwitcher(SynchronizationContext newContext)
        {
            this.PreviousContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(newContext);
        }

        /// <summary>
        /// The previous <see cref="SynchronizationContext"/>.
        /// </summary>
        private SynchronizationContext PreviousContext { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SynchronizationContext.SetSynchronizationContext(this.PreviousContext);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Executes a synchronous delegate without the current <see cref="SynchronizationContext"/>. The current context is restored when this function returns.
        /// </summary>
        public static SynchronizationContextSwitcher NoContext()
            => new SynchronizationContextSwitcher(null);
    }
}
