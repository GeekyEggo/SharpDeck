namespace SharpDeck
{
    using System.Linq;

    /// <summary>
    /// Provides helper methods for generating hash codes.
    /// </summary>
    internal static class HashCode
    {
        /// <summary>
        /// Combines the specified values to form a hash code; this is an adaptation of <see href="https://stackoverflow.com/a/1646913/259656"/>.
        /// </summary>
        /// <param name="values">The values to combine.</param>
        /// <returns>The hash code.</returns>
        public static int Combine(params object[] values)
            => values.Aggregate(17, (hash, value) => hash * 31 + value.GetHashCode());
    }
}
