namespace StreamDeck.Manifest.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides extension methods for <see cref="AttributeData"/>.
    /// </summary>
    internal static class AttributeDataExtensions
    {
        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/> from the <paramref name="data"/>.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="data">The attribute data.</param>
        /// <returns>The newly constructed attribute.</returns>
        internal static T CreateInstance<T>(this AttributeData data)
        {
            var attr = (T)Activator.CreateInstance(typeof(T), data.ConstructorArguments.Select(x => x.Value).ToArray());
            data.Populate(attr);

            return attr;
        }

        /// <summary>
        /// Populates the specified <paramref name="obj"/> from the <paramref name="data"/>.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="data">The data used to populate the object.</param>
        /// <param name="obj">The object to populate.</param>
        /// <returns>The populated object.</returns>
        internal static T Populate<T>(this AttributeData data, T obj)
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite)
                .ToDictionary(p => p.Name, p => p);

            foreach (var namedArgument in data.NamedArguments)
            {
                if (properties.TryGetValue(namedArgument.Key, out var property))
                {
                    if (property.PropertyType.IsArray)
                    {
                        var elementType = property.PropertyType.GetElementType();
                        property.SetValue(obj, namedArgument.Value.Values.Select(x => x.Value).CastArray(elementType));
                    }
                    else
                    {
                        property.SetValue(obj, namedArgument.Value.Value);
                    }
                }
            }

            return obj;
        }
    }
}
