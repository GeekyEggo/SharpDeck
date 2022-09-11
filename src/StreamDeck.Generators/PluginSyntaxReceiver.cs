namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck;
    using StreamDeck.Generators.Extensions;

    /// <summary>
    /// Provides a <see cref="ISyntaxContextReceiver"/> that is capable of discovering information relating to a Stream Deck plugin.
    /// </summary>
    internal class PluginSyntaxReceiver : ISyntaxContextReceiver
    {
        /// <summary>
        /// Gets the Stream Deck actions.
        /// </summary>
        public List<ActionClassContext> Actions { get; } = new List<ActionClassContext>();

        /// <summary>
        /// Gets the <see cref="AttributeSyntax"/> that represents the <see cref="StreamDeck.ManifestAttribute"/>.
        /// </summary>
        public AttributeSyntax? ManifestAttribute { get; private set; }

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is AttributeSyntax attrNode
                && context.SemanticModel.GetTypeInfo(context.Node).Type?.ToDisplayString(SymbolDisplayFormats.FullName) == "StreamDeck.ManifestAttribute")
            {
                this.ManifestAttribute = attrNode;
            }
            else if (context.Node is ClassDeclarationSyntax classNode
                && context.SemanticModel.GetDeclaredSymbol(context.Node) is INamedTypeSymbol symbol)
            {
                var attrs = classNode.GetAttributes(symbol);
                if (attrs.GetAttributesOfType<ActionAttribute>().ToArray() is { Length: > 0 } actionAttrs)
                {
                    var actionClassContext = new ActionClassContext(classNode, symbol, attrs[0]);
                    actionClassContext.StateAttributes.AddRange(attrs.GetAttributesOfType<StateAttribute>());

                    this.Actions.Add(actionClassContext);
                }
            }
        }
    }
}
