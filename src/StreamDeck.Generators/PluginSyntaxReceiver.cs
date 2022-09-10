namespace StreamDeck.Generators
{
    using System.Collections.ObjectModel;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.Models;

    /// <summary>
    /// Provides a <see cref="ISyntaxContextReceiver"/> that is capable of discovering information relating to a Stream Deck plugin.
    /// </summary>
    internal class PluginSyntaxReceiver : ISyntaxContextReceiver
    {
        /// <summary>
        /// Gets the class nodes that represent Stream Deck actions.
        /// </summary>
        public Collection<ActionClassDeclarationSyntax> ActionNodes { get; } = new Collection<ActionClassDeclarationSyntax>();

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclaration)
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
                if (symbol is not null
                    && symbol.TryGetAttribute<ActionAttribute>(out var attr) == true)
                {
                    this.ActionNodes.Add(new ActionClassDeclarationSyntax(classDeclaration, symbol, attr));
                }
            }
        }
    }
}
