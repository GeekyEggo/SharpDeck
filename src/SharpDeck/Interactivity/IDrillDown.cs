namespace SharpDeck.Interactivity
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides functionality for showing the items of type <typeparamref name="T"/> as part of a drill down.
    /// </summary>
    /// <typeparam name="T">The type of the items within the drill down.</typeparam>
    public interface IDrillDown<T>
    {
        /// <summary>
        /// Closes the drill down, and switches back to the previous profile.
        /// </summary>
        void Close();

        /// <summary>
        /// Closes the drill down, and switches back to the previous profile; the result of the drill down is set to the specified <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result of the drill down.</param>
        void CloseWithResult(T result);

        /// <summary>
        /// Shows the drill-down with the given items asynchronously.
        /// </summary>
        /// <param name="items">The items to show.</param>
        /// <returns>The result of the drill down.</returns>
        Task<DrillDownResult<T>> ShowAsync(IEnumerable<T> items);
    }
}
