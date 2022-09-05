namespace StreamDeck.Generators.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides extension methods for <see cref="TypedConstant"/>.
    /// </summary>
    internal static class TypedConstantExtensions
    {
        /// <summary>
        /// Casts the underlying collection of values to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The desired value type.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The enumerable of values cast as <typeparamref name="T"/>.</returns>
        internal static IEnumerable<T?> AsEnumerable<T>(this TypedConstant value)
            => value.Values.Select(x => (T?)x.Value);
    }
}
