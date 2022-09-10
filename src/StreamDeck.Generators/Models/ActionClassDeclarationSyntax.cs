namespace StreamDeck.Generators.Models
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
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
            this.Symbol = symbol;

            this.IsUuidValid = !Regex.IsMatch(this.Action.UUID, @"[^a-z0-9\-\.]+");
        }

        /// <summary>
        /// Gets the <see cref="ActionAttribute"/> that represents the action.
        /// </summary>
        public ActionAttribute Action { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ActionAttribute.UUID"/> is valid according to <see href="https://developer.elgato.com/documentation/stream-deck/sdk/manifest/"/>.
        /// </summary>
        public bool IsUuidValid { get; }

        /// <summary>
        /// Gets the locations.
        /// </summary>
        public IEnumerable<Location> Locations { get; }

        /// <summary>
        /// Gets the collection of <see cref="StateAttribute"/> associated with the action.
        /// </summary>
        public StateAttribute[] States { get; }

        /// <summary>
        /// Gets the symbol that represents the class declaration node.
        /// </summary>
        public ISymbol Symbol { get; }
    }
}
