namespace SharpDeck.Extensions
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides extension methods for <see cref="Task"/>.
    /// </summary>
    internal static class TaskExtensions
    {
        /// <summary>
        /// The task is executed on a separate synchronization context. Exceptions are logged to the <paramref name="logger"/>; <see cref="OperationCanceledException"/> are logged as <see cref="LogLevel.Debug"/>.
        /// </summary>
        /// <param name="task">The task; this instance..</param>
        /// <param name="logger">The logger.</param>
        public static async void Forget(this Task task, ILogger logger)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                logger?.LogDebug(ex, "A forgotten task was cancelled.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "A forgotten task threw an exception.");
            }
        }
    }
}
