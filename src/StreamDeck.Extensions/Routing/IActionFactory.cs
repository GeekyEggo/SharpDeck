namespace StreamDeck.Extensions.Routing
{
    using System;

    /// <summary>
    /// Provides a factory capable of creating a <see cref="StreamDeckAction"/> from a <see cref="Type"/> and <see cref="ActionInitializationContext"/>.
    /// </summary>
    internal interface IActionFactory
    {
        /// <summary>
        /// Creates a new instance of an <see cref="StreamDeckAction"/> of the specified <paramref name="actionType"/>.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="context">The action's initialization context.</param>
        /// <returns>The new instance of the <see cref="StreamDeckAction"/>.</returns>
        StreamDeckAction CreateInstance(Type actionType, ActionInitializationContext context);
    }
}
