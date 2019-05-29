namespace SharpDeck
{
    using SharpDeck.Events;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an implementation of an action, containing settings, that can be registered on a <see cref="StreamDeckClient"/>.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    public class StreamDeckAction<TSettings> : StreamDeckAction
        where TSettings : class
    {
        /// <summary>
        /// Gets this action's instances settings asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <returns>The task containing the settings.</returns>
        public Task<TSettings> GetSettingsAsync()
        {
            var taskSource = new TaskCompletionSource<TSettings>();

            // declare the local function handler that sets the task result
            void handler(object sender, ActionEventArgs<ActionPayload> e)
            {
                this.DidReceiveSettings -= handler;
                taskSource.TrySetResult(e.Payload.GetSettings<TSettings>());
            }

            // listen for receiving events, and trigger a request
            this.DidReceiveSettings += handler;
            this.StreamDeck.GetSettingsAsync(this.Context);

            return taskSource.Task;
        }

        /// <summary>
        /// Save persistent data for the actions instance.
        /// </summary>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        public Task SetSettingsAsync(TSettings settings)
            => this.StreamDeck.SetSettingsAsync(this.Context, settings);
    }
}
