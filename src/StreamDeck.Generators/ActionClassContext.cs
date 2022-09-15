namespace StreamDeck.Generators
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck.Generators.Analyzers;

    /// <summary>
    /// Provides information about a class, and its attributes, that represent a Stream Deck action.
    /// </summary>
    internal struct ActionClassContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionClassContext"/> struct.
        /// </summary>
        /// <param name="symbol">The <see cref="INamedTypeSymbol"/> of the class declaration node.</param>
        /// <param name="actionAttribute">The attribute that contains information about the <see cref="StreamDeck.ActionAttribute"/>.</param>
        internal ActionClassContext(ClassDeclarationSyntax node, INamedTypeSymbol symbol, AttributeContext actionAttribute)
        {
            this.ActionAttribute = actionAttribute;
            this.IsPartial = node.Modifiers.Any(SyntaxKind.PartialKeyword);
            this.Symbol = symbol;
        }

        /// <summary>
        /// Gets the <see cref="AttributeContext"/> that contains information about the <see cref="StreamDeck.ActionAttribute"/>.
        /// </summary>
        internal AttributeContext ActionAttribute { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the class is a partial class.
        /// </summary>
        internal bool IsPartial { get; set; } = false;

        /// <summary>
        /// Gets the <see cref="ActionAttribute.Name"/>.
        /// </summary>
        internal string? Name => this.ActionAttribute.Data.ConstructorArguments[0].Value?.ToString();

        /// <summary>
        /// Gets the <see cref="ActionAttribute.UUID"/>.
        /// </summary>
        internal string? UUID => this.ActionAttribute.Data.ConstructorArguments[1].Value?.ToString();

        /// <summary>
        /// Gets the collection of <see cref="AttributeContext"/> that represent <see cref="StreamDeck.StateAttribute"/>.
        /// </summary>
        internal List<AttributeContext> StateAttributes { get; } = new List<AttributeContext>();

        /// <summary>
        /// Gets the <see cref="ISymbol"/> of the class declaration node.
        /// </summary>
        internal INamedTypeSymbol Symbol { get; }
    }
}
