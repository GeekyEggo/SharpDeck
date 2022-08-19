namespace StreamDeck.Events
{
    using System;

    /// <summary>
    /// Provides payload information containing settings.
    /// </summary>
    public class SettingsPayload
    {
        /// <summary>
        /// Gets the JSON containing data that you can set and are stored persistently.
        /// </summary>
        public object? Settings { get; internal set; }

        /// <summary>
        /// Gets the settings as the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The desired type of the settings.</typeparam>
        /// <returns>The settings as <typeparamref name="T"/>.</returns>
        public T GetSettings<T>()
            where T : class
            // TODO: Update this to use System.Text.Json.
            => throw new NotImplementedException();
    }
}
