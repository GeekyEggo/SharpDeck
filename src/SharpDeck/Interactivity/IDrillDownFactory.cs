namespace SharpDeck.Interactivity
{
    /// <summary>
    /// Provides a factory for creating <see cref="DrillDown{TManager, TItem}"/>.
    /// </summary>
    internal interface IDrillDownFactory
    {
        /// <summary>
        /// Creates a new <see cref="DrillDown{TManager, TItem}"/>.
        /// </summary>
        /// <typeparam name="TManager">The type of the drill-down manager.</typeparam>
        /// <typeparam name="TItem">The type of the items the manager is capable of handling.</typeparam>
        /// <param name="deviceUUID">The device UUID the drill-down is for.</param>
        /// <returns>The drill-down.</returns>
        IDrillDown<TManager, TItem> Create<TManager, TItem>(string deviceUUID)
            where TManager : class, IDrillDownManager<TItem>;
    }
}
