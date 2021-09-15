namespace SharpDeck.Interactivity
{
    /// <summary>
    /// Provides information about the result of a <see cref="IDrillDown{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items within the drill down.</typeparam>
    public struct DrillDownResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrillDownResult{T}"/> struct.
        /// </summary>
        /// <param name="isSuccess">The value indicating whether a selection was successful; when <c>true</c> the <see cref="Item"/> was the item selected by the user.</param>
        /// <param name="item">The item selected by the user.</param>
        internal DrillDownResult(bool isSuccess, T item)
        {
            this.IsSuccess = isSuccess;
            this.Item = item;
        }

        /// <summary>
        /// Gets the drill down result that represents no result.
        /// </summary>
        public static DrillDownResult<T> None => new DrillDownResult<T>(false, default);

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
