namespace SharpDeck.Interactivity
{
    /// <summary>
    /// Provides information about the result of a <see cref="IDynamicProfile{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items within the dynamic profile.</typeparam>
    public struct DynamicProfileResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProfileResult{T}"/> struct.
        /// </summary>
        /// <param name="isSuccess">The value indicating whether a selection was successful; when <c>true</c> the <see cref="Item"/> was the item selected by the user.</param>
        /// <param name="item">The item selected by the user.</param>
        internal DynamicProfileResult(bool isSuccess, T item)
        {
            this.IsSuccess = isSuccess;
            this.Item = item;
        }

        /// <summary>
        /// Gets the dynamic profile result that represents no result.
        /// </summary>
        public static DynamicProfileResult<T> None => new DynamicProfileResult<T>(false, default);

        /// <summary>
        /// Gets a value indicating whether a selection was successful; when <c>true</c> the <see cref="Item"/> was the item selected by the user.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the item selected by the user.
        /// </summary>
        public T Item { get; }
    }
}
