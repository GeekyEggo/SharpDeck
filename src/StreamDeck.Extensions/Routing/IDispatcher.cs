namespace StreamDeck.Extensions.Routing
{
    /// <summary>
    /// Provides methods for dispatching actions.
    /// </summary>
    internal interface IDispatcher
    {
        /// <summary>
        /// Invokes the <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="context">The context of the invocation.</param>
        void Invoke(Func<Task> action, string context = "");
    }
}
