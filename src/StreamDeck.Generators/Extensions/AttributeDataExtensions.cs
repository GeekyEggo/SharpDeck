namespace StreamDeck.Generators.Extensions
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
        public static T CreateInstance<T>(this AttributeData data)
        {
            var attr = (T)Activator.CreateInstance(typeof(T), data.ConstructorArguments.Select(x => x.Value).ToArray());
            data.Populate(attr);

            return attr;
        }

        /// <summary>
        /// Gets the named argument value as a <see cref="string"/>, otherwise the result of <paramref name="defaultFactory" />.
        /// </summary>
        /// <param name="data">The <see cref="AttributeData"/> that contains the named arguments.</param>
        /// <param name="name">The name of the named argument whose value should be retrieved.</param>
        /// <param name="defaultFactory">The factory responsible for creating the default value.</param>
        /// <returns>The value of the named argument; otherwise the result of <paramref name="defaultFactory"/></returns>
        public static string GetNamedArgumentValueOrDefault(this AttributeData data, string name, Func<string> defaultFactory)
            => data.TryGetNamedArgument(name, out string? value) && !string.IsNullOrWhiteSpace(value)
                ? value!
                : defaultFactory();

        /// <summary>
        /// Populates the specified <paramref name="obj"/> from the <paramref name="data"/>.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="data">The data used to populate the object.</param>
        /// <param name="obj">The object to populate.</param>
        /// <returns>The populated object.</returns>
        public static T Populate<T>(this AttributeData data, T obj)
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
                    else if (property.PropertyType != typeof(Type))
                    {
                        property.SetValue(obj, namedArgument.Value.Value);
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Tries the get the frist named argument that matches the <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the argument value.</typeparam>
        /// <param name="data">The <see cref="AttributeData"/> whose arguments should be searched.</param>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The result value of the argument.</param>
        /// <returns><c>true</c> when a named argument was found that matches the <paramref name="name"/>, and has a value of type <typeparamref name="T"/>; otherwise <c>false</c>.</returns>
        public static bool TryGetNamedArgument<T>(this AttributeData data, string name, out T? value)
        {
            if (data.NamedArguments.FirstOrDefault(a => a.Key == name) is KeyValuePair<string, TypedConstant> arg
                && arg.Key == name
                && arg.Value.Value is T _value)
            {
                value = _value;
                return true;
            }

            value = default;
            return false;
        }
    }
}
