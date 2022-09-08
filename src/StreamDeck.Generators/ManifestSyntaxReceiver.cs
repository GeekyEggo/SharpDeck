namespace StreamDeck.Generators
{
    using System.Collections.ObjectModel;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.Validators;

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
        /// Gets the <see cref="ActionAttribute"/> associated with the compilation, and returns <c>true</c> when all actions are valid.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="actions">The actions.</param>
        /// <returns><c>true</c> when all actions are valid; otherwise <c>false</c>.</returns>
        internal bool TryGetActions(GeneratorExecutionContext context, out List<ActionAttribute> actions)
        {
            var isValid = true;
            var validator = new ActionValidator(context);

            actions = new List<ActionAttribute>();
            foreach (var actionDeclaration in this.Actions)
            {
                var location = actionDeclaration.Symbol.Locations.First();

                var action = actionDeclaration.AttributeData.CreateInstance<ActionAttribute>();
                var states = actionDeclaration.Symbol.GetAttributes<StateAttribute>().Select(s => s.CreateInstance<StateAttribute>()).ToArray();

                if (validator.Validate(action, states, location))
                {
                    action.States = states.Length > 0 ? states : action.States;
                    actions.Add(action);
                }
                else
                {
                    isValid = false;
                }
            }

            return isValid;
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
