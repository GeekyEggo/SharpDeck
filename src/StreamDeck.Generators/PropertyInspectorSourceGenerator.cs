namespace StreamDeck.Generators
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.PropertyInspectors;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides a <see cref="ISourceGenerator"/> capable of generating property inspector HTML files.
    /// </summary>
    internal class PropertyInspectorSourceGenerator : BaseSourceGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorSourceGenerator"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        public PropertyInspectorSourceGenerator(IFileSystem fileSystem)
            => this.FileSystem = fileSystem;

        /// <summary>
        /// Gets the file system.
        /// </summary>
        private IFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the component writers.
        /// </summary>
        private IReadOnlyDictionary<string, InputWriter> ComponentWriters { get; } = new Dictionary<string, InputWriter>
        {
            { typeof(CalendarAttribute).FullName, new CalendarInputWriter() },
            { typeof(CheckboxAttribute).FullName, new CheckboxInputWriter() },
            // Checkbox List
            { typeof(ColorAttribute).FullName, new InputWriter("sdpi-color") },
            { typeof(FileAttribute).FullName, new FileInputWriter() },
            { typeof(PasswordAttribute).FullName, new InputWriter("sdpi-password") },
            // Radio
            { typeof(RangeAttribute).FullName, new RangeInputWriter() },
            { typeof(SelectAttribute).FullName, new SelectInputWriter() },
            { typeof(TextareaAttribute).FullName, new InputWriter("sdpi-textarea") },
            { typeof(TextfieldAttribute).FullName, new TextfieldInputWriter() }
        };

        /// <inheritdoc/>
        internal override void Execute(GeneratorExecutionContext context, StreamDeckSyntaxReceiver syntaxReceiver, ManifestAnalyzer manifestAnalyzer)
        {
            if (!context.TryGetProjectDirectory(out var projectDirectory))
            {
                // todo: warn.
                throw new InvalidOperationException();
            }

            foreach (var actionAnalyzer in manifestAnalyzer.ActionAnalyzers.Where(a => a.PropertyInspectorType is not null))
            {
                this.FileSystem.WriteAllText(
                    path: Path.Combine(projectDirectory, actionAnalyzer.Action.PropertyInspectorPath),
                    contents: this.GetPropertyInspectorHtml(actionAnalyzer),
                    encoding: Encoding.UTF8);
            }
        }

        /// <summary>
        /// Gets the property inspector HTML for the <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action analysis.</param>
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
                    foreach (var prop in action.PropertyInspectorType!.GetMembers().OfType<IPropertySymbol>())
                    {
                        var propAttrs = prop.GetAttributes();
                        foreach (var attr in propAttrs)
                        {
                            if (attr.AttributeClass?.ToFullNameString() is string attrClass
                                && attrClass is not null
                                && this.ComponentWriters.TryGetValue(attrClass, out var componentWriter))
                            {
                                componentWriter!.Write(body, attr, propAttrs);
                            }
                        }
                    }
                });
            });

            return htmlWriter.ToString();
        }
    }
}
