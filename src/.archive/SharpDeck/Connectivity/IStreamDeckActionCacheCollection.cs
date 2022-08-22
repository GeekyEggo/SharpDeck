namespace SharpDeck.Connectivity
{
    using System;
    using System.Threading.Tasks;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides cache and validity management of <see cref="StreamDeckAction"/>.
    /// </summary>
    public interface IStreamDeckActionCacheCollection : IDisposable
    {
        /// <summary>
        /// Adds the specified <paramref name="action"/> against the <paramref name="key"/> asynchronously
        /// </summary>
        /// <param name="key">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        /// <param name="action">The action to cache.</param>
        /// <returns>The task of adding the item.</returns>
        Task AddAsync(ActionEventArgs<AppearancePayload> key, StreamDeckAction action);

        /// <summary>
        /// Attempts to get the cached <see cref="StreamDeckAction" /> for the specified <paramref name="key" />
        /// </summary>
        /// <param name="key">The <see cref="IActionEventArgs" /> instance containing the event data.</param>
        /// <param name="action">The result action.</param>
        /// <param name="payload">The optional payload containing the settings; when these are supplied, the cache item is validated.</param>
        /// <returns><c>true</c> when the action was found, and valid if <paramref name="payload"/> were supplied; otherwise <c>false</c>.</returns>
        bool TryGet(IActionEventArgs key, out StreamDeckAction action, SettingsPayload payload = null);
    }
}
