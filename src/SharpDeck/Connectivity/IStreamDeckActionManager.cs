namespace SharpDeck.Connectivity
{
    using System;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides connectivity between a <see cref="IStreamDeckConnection"/> and registered <see cref="StreamDeckAction"/>.
    /// </summary>
    internal interface IStreamDeckActionManager
    {
        /// <summary>
        /// Registers a new <see cref="StreamDeckAction" /> for the specified action UUID.
        /// </summary>
        /// <param name="actionUUID">The action UUID associated with the Stream Deck.</param>
        /// <param name="type">The underlying type of the <see cref="StreamDeckAction"/>.</param>
        void Register(string actionUUID, Type type);

        /// <summary>
        /// Attempts to get the action instance for the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">The <see cref="IActionEventArgs"/> instance containing the event data.</param>
        /// <param name="value">The <see cref="StreamDeckAction"/> instanced associated with the <paramref name="args"/>.</param>
        /// <returns><c>true</c> when an instance exists for the given <paramref name="args"/>; otherwise <c>false</c>.</returns>
        bool TryGet(IActionEventArgs args, out StreamDeckAction value);
    }
}
