namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.Generators.PropertyInspectors;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.PropertyInspectors;
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
                var html = this.GetPropertyInspectorHtml(action);
            }
        }

        /// <summary>
        /// Gets the property inspector HTML for the <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The HTML of the property inspector.</returns>
        private string GetPropertyInspectorHtml(ActionAnalyzer action)
        {
            var htmlWriter = new HtmlStringWriter();
            htmlWriter.Add("html", html =>
            {
                html.Add("head", head =>
                {

                    head.AddAttribute("lang", "en");
                    head.Add("meta", meta => meta.AddAttribute("charset", "utf-8"));
                    head.Add("script", script => script.AddAttribute("src", "https://cdn.jsdelivr.net/gh/geekyeggo/sdpi-components@v2/dist/sdpi-components.js"));
                });

                html.Add("body", body =>
                {
                    foreach (var propAttr in action.PropertyInspectorType!.GetMembers().OfType<IPropertySymbol>().SelectMany(p => p.GetAttributes()))
                    {
                        if (propAttr.AttributeClass?.ToFullNameString() is string attrClass
                            && attrClass is not null
                            && this.ComponentWriters.TryGetValue(attrClass, out var componentWriter))
                        {
                            componentWriter!.Write(body, propAttr);
                        }
                    }
                });
            });

            return htmlWriter.ToString();
        }
    }
}
