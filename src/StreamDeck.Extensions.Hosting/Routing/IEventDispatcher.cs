namespace StreamDeck.Routing
{
    using StreamDeck.Events;

    /// <summary>
    /// Provides methods for dispatching events.
    /// </summary>
    internal interface IEventDispatcher
    {
        /// <summary>
        /// Invokes the <paramref name="event"/> with the specified <typeparamref name="TArgs"/>.
        /// </summary>
        /// <typeparam name="TArgs">The type of the arguments.</typeparam>
        /// <param name="event">The event to invoke.</param>
        /// <param name="args">The arguments to supply to the <paramref name="event"/>.</param>
        void Invoke<TArgs>(Func<TArgs, Task> @event, TArgs args)
            where TArgs : IActionContext;
    }
}
