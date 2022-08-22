namespace SharpDeck.Extensions
{
    using System;

    /// <summary>
    /// Provides extension methods for <see cref="Type"/>.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether an instance of the specified <paramref name="type" /> can be assigned to an instance of this instance.
        /// </summary>
        /// <param name="source">The source type.</param>
        /// <param name="type">The type to compare against.</param>
        /// <returns><c>true</c> when the <paramref name="type"/> is derived from this instance; otherwise <c>false</c>.</returns>
        public static bool IsGenericAssignableFrom(this Type source, Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType
                    && type.GetGenericTypeDefinition() == source)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}
