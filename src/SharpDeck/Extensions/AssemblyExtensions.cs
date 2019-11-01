namespace SharpDeck.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="Assembly"/>.
    /// </summary>
    internal static class AssemblyExtensions
    {
        /// <summary>
        /// Gets types from this assembly, which have the specified custom attribute.
        /// </summary>
        /// <typeparam name="T">The type of attribute.</typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The types.</returns>
        public static IEnumerable<(Type type, T attribute)> GetTypesWithCustomAttribute<T>(this Assembly assembly) where T : Attribute
        {
            foreach (var type in assembly.GetSafeTypes())
            {
                var attribute = type.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    yield return (type, attribute);
                }
            }
        }

        /// <summary>
        /// Gets the types defined in this assembly, that can be loaded.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The defined and loaded types.</returns>
        public static IEnumerable<Type> GetSafeTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
