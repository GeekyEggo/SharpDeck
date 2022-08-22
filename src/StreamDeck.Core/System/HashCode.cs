#if NETSTANDARD2_0
namespace System
{
    using System.Linq;

    /// <summary>
    /// Provides helper methods for generating hash codes.
    /// </summary>
    internal struct HashCode
    {
        /// <summary>
        /// Combines the specified values to form a hash code; this is an adaptation of <see href="https://stackoverflow.com/a/1646913/259656"/>.
        /// </summary>
        /// <param name="values">The values to combine.</param>
        /// <returns>The hash code.</returns>
        public static int Combine(params object?[] values)
        {
            if (values != null)
            {
                values.Aggregate(17, (hash, value) => hash * 31 + value?.GetHashCode() ?? 0);
            }

            return 0;
        }
    }
}
#endif
