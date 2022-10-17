namespace StreamDeck.Generators.Extensions
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
        public static IEnumerable<AttributeData> GetAttributes<TAttribute>(this ISymbol symbol)
        {
            var typeName = typeof(TAttribute).FullName;

            return symbol
                .GetAttributes()
                .Where(a => a.AttributeClass?.ToString() == typeName);
        }

        /// <summary>
        /// Gets the fully qualified name of the type, including its namespace but not its assembly.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <returns>The full name of the <see cref="ISymbol"/>; otherwise <c>null</c>.</returns>
        public static string? ToFullNameString(this ISymbol? symbol)
            => symbol?.ToDisplayString(
                new SymbolDisplayFormat(
                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers));

        /// <summary>
        /// Attempts to get the first attribute that matches the type <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="symbol">The symbol.</param>
        /// <param name="attribute">The attribute; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> when an attribute of type <typeparamref name="TAttribute"/> was found; otherwise <c>false</c>.</returns>
        public static bool TryGetAttribute<TAttribute>(this ISymbol symbol, out AttributeData attribute)
        {
            attribute = symbol.GetAttributes<TAttribute>().FirstOrDefault();
            return attribute != null;
        }
    }
}
