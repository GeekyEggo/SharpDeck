namespace SharpDeck.Interactivity
{
    /// <summary>
    /// Provides a factory for creating <see cref="DynamicProfile{T}"/>.
    /// </summary>
    internal interface IDynamicProfileFactory
    {
        /// <summary>
        /// Creates a new <see cref="DynamicProfile{TItem}"/>.
        /// </summary>
        /// <typeparam name="TController">The type of the dynamic profile controller.</typeparam>
        /// <typeparam name="TItem">The type of the items within the dynamic profile.</typeparam>
        /// <param name="deviceUUID">The device UUID the dynamic profile is for.</param>
        /// <returns>The dynamic profile.</returns>
        IDynamicProfile<TItem> Create<TController, TItem>(string deviceUUID)
            where TController : class, IDynamicProfileController<TItem>;
    }
}
