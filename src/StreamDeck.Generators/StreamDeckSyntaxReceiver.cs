namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck.Generators.CodeAnalysis;
    using StreamDeck.Generators.Extensions;

    /// <summary>
    /// Provides a <see cref="ISyntaxContextReceiver"/> that is capable of discovering information relating to a Stream Deck plugin.
    /// </summary>
    internal class StreamDeckSyntaxReceiver : ISyntaxContextReceiver
    {
        /// <summary>
        /// Gets the Stream Deck actions.
        /// </summary>
        public List<ActionClassContext> Actions { get; } = new List<ActionClassContext>();

        /// <summary>
        /// Gets the <see cref="AttributeSyntax"/> that represents the <see cref="StreamDeck.ManifestAttribute"/>.
        /// </summary>
        public AttributeSyntax? ManifestAttribute { get; private set; }

        /// <summary>
        /// Gets the <see cref="AttributeSyntax"/> that represent the <see cref="StateAttribute"/>.
        /// </summary>
        public List<AttributeSyntax> ProfileAttributes { get; } = new List<AttributeSyntax>();

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is AttributeSyntax attrNode)
            {
                switch (context.SemanticModel.GetTypeInfo(context.Node).Type?.ToFullNameString())
                {
                    case string type when typeof(ManifestAttribute).FullName == type:
                        this.ManifestAttribute = attrNode;
                        break;

                    case string type when typeof(ProfileAttribute).FullName == type:
                        this.ProfileAttributes.Add(attrNode);
                        break;
                }
            }
            else if (context.Node is ClassDeclarationSyntax classNode
                && context.SemanticModel.GetDeclaredSymbol(context.Node) is INamedTypeSymbol symbol
                && this.TryGetActionClassContext(classNode, symbol, out var actionClassContext))
            {
                this.Actions.Add(actionClassContext);
            }
        }

        /// <summary>
        /// Gets the <paramref name="actionClassContext"/> from the specified <paramref name="classNode"/> and <paramref name="symbol"/>.
        /// </summary>
        /// <param name="classNode">The <see cref="ClassDeclarationSyntax"/>.</param>
        /// <param name="symbol">The <see cref="INamedTypeSymbol"/>.</param>
        /// <param name="actionClassContext">The <see cref="ActionClassContext"/> when the <paramref name="classNode"/> and <paramref name="symbol"/> were decorated with <see cref="ActionAttribute"/>.</param>
        /// <returns><c>true</c> when the <paramref name="actionClassContext"/> was constructed; otherwise <c>false</c>.</returns>
        private bool TryGetActionClassContext(ClassDeclarationSyntax classNode, INamedTypeSymbol symbol, out ActionClassContext actionClassContext)
        {
            var hasActionAttribute = false;
            actionClassContext = new ActionClassContext(classNode, symbol);

            var attributeNodes = classNode.AttributeLists.SelectMany(attrList => attrList.Attributes).ToArray();
            var attributeDatas = symbol.GetAttributes();

            foreach (var node in attributeNodes)
            {
                var nodeRef = node.GetReference();
                var data = attributeDatas.First(d => SyntaxReferenceEqualityComparer.Default.Equals(d.ApplicationSyntaxReference, nodeRef));

                switch (data.AttributeClass?.ToFullNameString())
                {
                    case string type when typeof(ActionAttribute).FullName == type && !hasActionAttribute:
                        actionClassContext.ActionAttribute = new AttributeContext(node, data);
                        hasActionAttribute = true;
                        break;

                    case string type when typeof(StateAttribute).FullName == type:
                        actionClassContext.StateAttributes.Add(new AttributeContext(node, data));
                        break;
                }
            }

            return hasActionAttribute;
        }
    }
}
