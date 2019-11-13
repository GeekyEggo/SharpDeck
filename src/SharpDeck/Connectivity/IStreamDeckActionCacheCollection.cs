namespace SharpDeck.Connectivity
{
    using System;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides cache and validity management of <see cref="StreamDeckAction"/>.
    /// </summary>
    public interface IStreamDeckActionCacheCollection : IDisposable
    {
        /// <summary>
        /// Adds the specified <paramref name="action"/> against the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        /// <param name="action">The action to cache.</param>
        void Add(ActionEventArgs<AppearancePayload> key, StreamDeckAction action);

        /// <summary>
        /// Attempts to get the cached <see cref="StreamDeckAction" /> for the specified <paramref name="key" />
        /// </summary>
        /// <param name="key">The <see cref="IActionEventArgs" /> instance containing the event data.</param>
        /// <param name="action">The result action.</param>
        /// <returns><c>true</c> when the action was found; otherwise <c>false</c>.</returns>
        bool TryGet(IActionEventArgs key, out StreamDeckAction action);

        /// <summary>
        /// Attempts to get the cached <see cref="StreamDeckAction" /> for the specified <paramref name="key" />
        /// </summary>
        /// <param name="key">The <see cref="IActionEventArgs" /> instance containing the event data.</param>
        /// <param name="action">The result action.</param>
        /// <param name="payload">The payload containing the settings allowing for the the cached item to be validated.</param>
        /// <returns><c>true</c> when the action was found, and valid; otherwise <c>false</c>.</returns>
        bool TryGet<T>(IActionEventArgs key, out StreamDeckAction action, T payload)
            where T : SettingsPayload;
    }
}
