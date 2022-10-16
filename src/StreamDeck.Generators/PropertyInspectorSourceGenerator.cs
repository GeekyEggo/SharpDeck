namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck.Generators.Analyzers;
    using StreamDeck.Generators.Generators.PropertyInspectors;
    using StreamDeck.Generators.PropertyInspectors;
    using StreamDeck.Generators.Serialization;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides a <see cref="ISourceGenerator"/> capable of generating property inspector HTML files.
    /// </summary>
    internal class PropertyInspectorSourceGenerator : BaseSourceGenerator
    {
        /// <summary>
        /// Gets the component writers.
        /// </summary>
        private IReadOnlyDictionary<string, FieldItemWriter> ComponentWriters { get; } = new Dictionary<string, FieldItemWriter>
        {
            { typeof(TextfieldAttribute).FullName, new TextfieldWriter() }
        };

        /// <inheritdoc/>
        internal override void Execute(GeneratorExecutionContext context, StreamDeckSyntaxReceiver syntaxReceiver, ManifestAnalyzer manifestAnalyzer)
        {
            foreach (var piAttr in this.GetPropertyInspectors(context, syntaxReceiver))
            {
                if (piAttr.ConstructorArguments[0] is TypedConstant arg
                    && arg.Value is INamedTypeSymbol piType)
                {
                    using var htmlWriter = new HtmlTextWriter();
                    foreach (var propAttr in piType.GetMembers().OfType<IPropertySymbol>().SelectMany(p => p.GetAttributes()))
                    {
                        var attrClass = propAttr.AttributeClass?.ToDisplayString(SymbolDisplayFormats.FullName) ?? string.Empty;
                        if (this.ComponentWriters.TryGetValue(attrClass, out var writer))
                        {
                            writer.Write(htmlWriter, propAttr);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="PropertyInspectorAttribute"/> data defined within the <see cref="StreamDeckSyntaxReceiver"/>.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="syntaxReceiver">The <see cref="StreamDeckSyntaxReceiver"/> from the <paramref name="context"/>.</param>
        /// <returns>The collection of <see cref="PropertyInspectorAttribute"/> data.</returns>
        private IEnumerable<AttributeData> GetPropertyInspectors(GeneratorExecutionContext context, StreamDeckSyntaxReceiver syntaxReceiver)
        {
            foreach (var pi in syntaxReceiver.PropertyInspectors)
            {
                var semanticModel = context.Compilation.GetSemanticModel(pi.SyntaxTree);
                if (TryGetParent(semanticModel, pi, out var classSymbol)
                    && classSymbol!.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString(SymbolDisplayFormats.FullName) == typeof(PropertyInspectorAttribute).FullName) is AttributeData attrData)
                {
                    yield return attrData;
                }
            }

            // Gets the parent class declaration associated with the node.
            static bool TryGetParent(SemanticModel? semanticModel, SyntaxNode? node, out INamedTypeSymbol? classSymbol)
            {
                if (semanticModel is null
                    || node is null)
                {
                    classSymbol = null;
                    return false;
                }

                if (node is ClassDeclarationSyntax classNode
                    && semanticModel.GetDeclaredSymbol(classNode) is INamedTypeSymbol symbol)
                {
                    classSymbol = symbol;
                    return true;
                }

                return TryGetParent(semanticModel, node.Parent, out classSymbol);
            }
        }
    }
}
