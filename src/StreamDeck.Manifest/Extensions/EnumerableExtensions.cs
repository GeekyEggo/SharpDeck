namespace StreamDeck.Manifest.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Casts the collection to the specified <paramref name="type"/>, and returns them as an array.
        /// </summary>
        /// <typeparam name="T">The type of element within the collection.</typeparam>
        /// <param name="source">The collection.</param>
        /// <param name="type">The desired type.</param>
        /// <returns>The array of casted elements.</returns>
        internal static IEnumerable CastArray<T>(this IEnumerable<T> source, Type type)
        {
            var method = typeof(EnumerableExtensions)
                .GetMethod(nameof(EnumerableExtensions.CastArrayInternal), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(type);

            return (IEnumerable)method.Invoke(null, new object[] { source });
        }

        /// <summary>
        /// Casts the instance to <typeparamref name="T"/>, and then converts it to an array.
        /// </summary>
        /// <typeparam name="T">The type of element within the source.</typeparam>
        /// <param name="source">The collection.</param>
        /// <returns>The array of casted elements.</returns>
        private static T[] CastArrayInternal<T>(this IEnumerable<object> source)
            => source.Cast<T>().ToArray();
    }
}
