namespace StreamDeck.Generators.Models
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Extensions;

    /// <summary>
    /// Provides information about a <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax"/> that represents a Stream Deck action.
    /// </summary>
    internal struct ActionClassDeclarationSyntax
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionClassDeclarationSyntax"/> struct.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="actionAttribute">The action attribute.</param>
        public ActionClassDeclarationSyntax(ISymbol symbol, AttributeData actionAttribute)
        {
            this.Action = actionAttribute.CreateInstance<ActionAttribute>();
            this.Locations = symbol.Locations;
            this.States = symbol.GetAttributes<StateAttribute>()
                .Select(a => a.CreateInstance<StateAttribute>()).ToArray();
        }

        /// <summary>
        /// Gets the <see cref="ActionAttribute"/> that represents the action.
        /// </summary>
        public ActionAttribute Action { get; }

        /// <summary>
        /// Gets the locations.
        /// </summary>
        public IEnumerable<Location> Locations { get; }

        /// <summary>
        /// Gets the collection of <see cref="StateAttribute"/> associated with the action.
        /// </summary>
        public StateAttribute[] States { get; }
    }
}
