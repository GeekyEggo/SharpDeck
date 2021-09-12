namespace SharpDeck.Connectivity
{
    using System;

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
    }
}
