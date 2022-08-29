namespace StreamDeck.Manifest.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides extension methods for <see cref="ISymbol"/>.
    /// </summary>
    internal static class SymbolExtensions
    {
        /// <summary>
        /// Gets attributes of type <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="symbol">The symbol.</param>
        /// <returns>The matching attributes.</returns>
        internal static IEnumerable<AttributeData> GetAttributes<TAttribute>(this ISymbol symbol)
        {
            var typeName = typeof(TAttribute).FullName;

            return symbol
                .GetAttributes()
                .Where(a => a.AttributeClass?.ToString() == typeName);
        }

        /// <summary>
        /// Gets the attribute value; otherwise <paramref name="default"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="symbol">The symbol.</param>
        /// <param name="default">The default fallback value.</param>
        /// <returns>The value of the attribute; otherwise <paramref name="default"/>.</returns>
        internal static TValue? GetAttributeValueOrDefault<TAttribute, TValue>(this ISymbol symbol, TValue? @default = default)
        {
            if (symbol.TryGetAttribute<TAttribute>(out var attribute)
                && attribute.ConstructorArguments.Length == 1)
            {
                return (TValue?)attribute.ConstructorArguments[0].Value;
            }

            return @default;
        }

        /// <summary>
        /// Attempts to get the first attribute that matches the type <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="symbol">The symbol.</param>
        /// <param name="attribute">The attribute; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> when an attribute of type <typeparamref name="TAttribute"/> was found; otherwise <c>false</c>.</returns>
        internal static bool TryGetAttribute<TAttribute>(this ISymbol symbol, out AttributeData attribute)
        {
            attribute = symbol.GetAttributes<TAttribute>().FirstOrDefault();
            return attribute != null;
        }
    }
}
