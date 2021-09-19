namespace SharpDeck.Interactivity
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides functionality for showing the items of type <typeparamref name="T"/> as part of a dynamic profile.
    /// </summary>
    /// <typeparam name="T">The type of the items within the dynamic profile.</typeparam>
    public interface IDynamicProfile<T>
    {
        /// <summary>
        /// Closes the dynamic profile, and switches back to the previous profile.
        /// </summary>
        void Close();

        /// <summary>
        /// Closes the dynamic profile, and switches back to the previous profile; the result of the dynamic profile is set to the specified <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result of the dynamic profile.</param>
        void CloseWithResult(T result);

        /// <summary>
        /// Shows the dynamic profile with the given items asynchronously.
        /// </summary>
        /// <param name="items">The items to show.</param>
        /// <returns>The result of the dynamic profile.</returns>
        Task<DynamicProfileResult<T>> ShowAsync(IEnumerable<T> items);
    }
}
