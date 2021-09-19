namespace SharpDeck.Connectivity
{
    using System.Reflection;

    /// <summary>
    /// Provides a registry of actions to be handled by the plugin.
    /// </summary>
    public interface IStreamDeckActionRegistry
    {
        /// <summary>
        /// Registers the specified <typeparamref name="T"/> action, with the given <paramref name="actionUUID"/>.
        /// </summary>
        /// <typeparam name="T">The type of the action.</typeparam>
        /// <param name="actionUUID">The action UUID.</param>
        /// <returns>The modified registry.</returns>
        IStreamDeckActionRegistry Register<T>(string actionUUID)
            where T : StreamDeckAction;

        /// <summary>
        /// Registers all instances of <see cref="StreamDeckAction" /> with the specified <paramref name="assembly" />; the action UUID is determined by the <see cref="StreamDeckActionAttribute.UUID" />
        /// </summary>
        /// <param name="assembly">The assembly to search in.</param>
        /// <returns>The modified registry.</returns>
        IStreamDeckActionRegistry RegisterAll(Assembly assembly);
    }
}
