namespace StreamDeck.Generators.CodeAnalysis
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public ActionClassContext(ClassDeclarationSyntax node, INamedTypeSymbol symbol)
        {
            this.IsPartial = node.Modifiers.Any(SyntaxKind.PartialKeyword);
            this.Symbol = symbol;
        }

        /// <summary>
        /// Gets the <see cref="AttributeContext"/> that contains information about the <see cref="StreamDeck.ActionAttribute"/>.
        /// </summary>
        public AttributeContext ActionAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the class is a partial class.
        /// </summary>
        public bool IsPartial { get; set; } = false;

        /// <summary>
        /// Gets the collection of <see cref="AttributeContext"/> that represent <see cref="StreamDeck.StateAttribute"/>.
        /// </summary>
        public List<AttributeContext> StateAttributes { get; } = new List<AttributeContext>();

        /// <summary>
        /// Gets the <see cref="ISymbol"/> of the class declaration node.
        /// </summary>
        public INamedTypeSymbol Symbol { get; }
    }
}
