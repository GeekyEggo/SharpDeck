namespace SharpDeck.Interactivity
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides drill-down functioanlity for the given <typeparamref name="TItem" /> using actions of type <typeparamref name="TManager" />.
    /// </summary>
    /// <typeparam name="TManager">The type of the drill-down manager.</typeparam>
    /// <typeparam name="TItem">The type of the items the manager is capable of handling.</typeparam>
    public interface IDrillDown<TManager, TItem>
        where TManager : class, IDrillDownManager<TItem>
    {
        /// <summary>
        /// Shows the drill-down with the given items asynchronously.
        /// </summary>
        /// <param name="items">The items to show.</param>
        /// <returns>The task of showing the items.</returns>
        Task ShowAsync(IEnumerable<TItem> items);
    }
}
