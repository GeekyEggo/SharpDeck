namespace SharpDeck.Interactivity
{
    /// <summary>
    /// Provides a factory for creating <see cref="DrillDown{T}"/>.
    /// </summary>
    public interface IDrillDownFactory
    {
        /// <summary>
        /// Creates a new <see cref="DrillDown{TItem}"/>.
        /// </summary>
        /// <typeparam name="TController">The type of the drill down controller.</typeparam>
        /// <typeparam name="TItem">The type of the items within the drill down.</typeparam>
        /// <param name="deviceUUID">The device UUID the drill down is for.</param>
        /// <returns>The drill down.</returns>
        IDrillDown<TItem> Create<TController, TItem>(string deviceUUID)
            where TController : class, IDrillDownController<TItem>;
    }
}
