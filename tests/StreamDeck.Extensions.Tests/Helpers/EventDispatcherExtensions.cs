namespace StreamDeck.Tests.Helpers
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using StreamDeck.Events;
    using StreamDeck.Routing;

    /// <summary>
    /// Provides extensions for <see cref="Mock{IEventDispatcher}"/>.
    /// </summary>
    internal static class EventDispatcherExtensions
    {
        /// <summary>
        /// Sets up the mock <see cref="IEventDispatcher"/> for the given <typeparamref name="TEventArgs"/>.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="dispatcher">The <see cref="Mock{IEventDispatcher}"/> to setup.</param>
        /// <returns>This <see cref="Mock{IEventDispatcher}"/> for chaining.</returns>
        internal static Mock<IEventDispatcher> SetupInvokeFor<TEventArgs>(this Mock<IEventDispatcher> dispatcher)
            where TEventArgs : IActionContext
        {
            dispatcher
                .Setup(d => d.Invoke(It.IsAny<Func<TEventArgs, Task>>(), It.IsAny<TEventArgs>()))
                .Callback<Func<TEventArgs, Task>, TEventArgs>((invoke, args) => invoke(args).Wait());

            return dispatcher;
        }
    }
}
