namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides a set of standard <see cref="SymbolDisplayFormat"/>.
    /// Credit to <see href="https://github.com/canton7/PropertyChanged.SourceGenerator/blob/master/src/PropertyChanged.SourceGenerator/SymbolDisplayFormats.cs"/>.
    /// </summary>
    internal static class SymbolDisplayFormats
    {
        /// <summary>
        /// Initializes the <see cref="SymbolDisplayFormats"/> class.
        /// </summary>
        static SymbolDisplayFormats()
        {
            FullName = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

            TypeDeclaration = new SymbolDisplayFormat(
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);
        }

        /// <summary>
        /// Gets the full name of a symbol, suitably escaped. E.g. "System.Collections.Generic"
        /// </summary>
        public static SymbolDisplayFormat FullName { get; }

        /// <summary>
        /// A class declaration, but not including accessibility or modifiers, e.g. "class List&lt;T&gt;"
        /// </summary>
        public static SymbolDisplayFormat TypeDeclaration { get; }
    }
}
