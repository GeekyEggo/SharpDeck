namespace StreamDeck.Generators
{
    using System.Collections.ObjectModel;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck;
    using StreamDeck.Generators.Extensions;

    /// <summary>
    /// Provides a <see cref="ISyntaxContextReceiver"/> that is capable of discovering information relative to generating a manifest file.
    /// </summary>
    internal class ManifestSyntaxReceiver : ISyntaxContextReceiver
    {
        /// <summary>
        /// Gets the classes that represent Stream Deck plugin actions.
        /// </summary>
        private Collection<(ISymbol Symbol, AttributeData AttributeData)> Actions { get; } = new Collection<(ISymbol Symbol, AttributeData AttributeData)>();

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax _)
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
                if (symbol?.TryGetAttribute<ActionAttribute>(out var attrData) == true)
                {
                    this.Actions.Add((symbol, attrData));
                }
            }
        }

        /// <summary>
        /// Gets the actions associated with compilation.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <returns>The actions discovered whilst traversing the compilation nodes.</returns>
        internal IEnumerable<ActionAttribute> GetActions(GeneratorExecutionContext context)
        {
            foreach (var actionDeclaration in this.Actions)
            {
                var action = actionDeclaration.AttributeData.CreateInstance<ActionAttribute>();
                var states = actionDeclaration.Symbol.GetAttributes<StateAttribute>().ToArray();

                // Validate UUID characters (https://developer.elgato.com/documentation/stream-deck/sdk/manifest/).
                if (Regex.IsMatch(action.UUID, @"[^a-z0-9\-\.]+"))
                {
                    context.ReportInvalidUUIDCharacters(actionDeclaration.Symbol.Locations.First());
                }

                if (states.Length > 0)
                {
                    // When there is a state image defined, and custom states, warn of duplication.
                    if (action.States.Count > 0)
                    {
                        context.ReportStateImageValueObsolete(actionDeclaration.Symbol.Locations.First());
                    }

                    action.States.Clear();
                    action.States.AddRange(states.Select(s => s.CreateInstance<StateAttribute>()).Take(2));
                }

                // Ensure we have at least 1 action state.
                if (action.States.Count == 0)
                {
                    context.ReportNoActionStatesDefined(actionDeclaration.Symbol.Locations.First());
                }

                // Ensure we dont have more than 2 action states.
                if (action.States.Count > 2)
                {
                    context.ReportTooManyActionStates(actionDeclaration.Symbol.Locations.First());
                }

                yield return action;
            }
        }

        /// <summary>
        /// Gets the profiles associated with compilation.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <returns>The profiles discovered from the context's compilation assembly.</returns>
        internal IEnumerable<ProfileAttribute> GetProfiles(GeneratorExecutionContext context)
        {
            foreach (var profileAttr in context.Compilation.Assembly.GetAttributes<ProfileAttribute>())
            {
                var item = new ProfileAttribute((string)profileAttr.ConstructorArguments[0].Value!, (Device)profileAttr.ConstructorArguments[1].Value!);
                yield return profileAttr.Populate(item);
            }
        }
    }
}
