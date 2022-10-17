namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
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
            foreach (var action in manifestAnalyzer.ActionAnalyzers.Where(a => a.PropertyInspectorType is not null))
            {
                using var htmlWriter = new HtmlTextWriter();
                foreach (var propAttr in action.PropertyInspectorType!.GetMembers().OfType<IPropertySymbol>().SelectMany(p => p.GetAttributes()))
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
}
