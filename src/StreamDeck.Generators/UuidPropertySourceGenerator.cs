namespace StreamDeck.Generators
{
    using System.CodeDom.Compiler;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Analyzers;

    /// <summary>
    /// Generates the const UUID property for classes with <see cref="ActionAttribute"/>.
    /// </summary>
    internal class UuidPropertySourceGenerator : BaseSourceGenerator
    {
        /// <summary>
        /// The UUID member name.
        /// </summary>
        private const string UUID_MEMBER_NAME = "UUID";

        /// <inheritdoc/>
        internal override void Execute(GeneratorExecutionContext context, StreamDeckSyntaxReceiver syntaxReceiver, ManifestAnalyzer manifestAnalyzer)
        {
            var hintNameIndexes = new Dictionary<string, int>();

            foreach (var actionAnalyzer in manifestAnalyzer.ActionAnalyzers.Where(CanAutoGenerate))
            {
                using var stringWriter = new StringWriter();
                using var source = new IndentedTextWriter(stringWriter);

                source.WriteLine("// <auto-generated />");
                source.WriteLine();

                // When we aren't on the global namespace, we should scope the class to the specified namespace
                if (!actionAnalyzer.Context.Symbol.ContainingNamespace.IsGlobalNamespace)
                {
                    source.WriteLine($"namespace {actionAnalyzer.Context.Symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.FullName)}");
                    source.WriteLine("{");
                    source.Indent++;

                    GenerateClassWithUuidProperty(source, actionAnalyzer);

                    source.Indent--;
                    source.WriteLine("}");
                }
                else
                {
                    GenerateClassWithUuidProperty(source, actionAnalyzer);
                }

                var hintName = actionAnalyzer.Context.Symbol.Name;
                hintNameIndexes[hintName] = hintNameIndexes.TryGetValue(hintName, out var index) ? ++index : 0;

                // Add the source.
                context.AddSource(
                    hintName: $"{hintName}.{index}.g",
                    stringWriter.ToString());
            }
        }

        /// <summary>
        /// Determines whether the action within the specified <paramref name="actionAnalyzer"/> can have its UUID property generated.
        /// </summary>
        /// <param name="actionAnalyzer">The <see cref="ActionAnalyzer"/>.</param>
        /// <returns><c>true</c> when the instance is valid, and the symbol associated with it is capable of having its UUID property generated; otherwise <c>false</c>.</returns>
        static bool CanAutoGenerate(ActionAnalyzer actionAnalyzer)
            => actionAnalyzer.HasValidUUID
            && actionAnalyzer.Context.IsPartial
            && actionAnalyzer.Context.Symbol is INamedTypeSymbol typedSymbol
            && !typedSymbol.MemberNames.Contains(UUID_MEMBER_NAME);

        /// <summary>
        /// Generates the partial class that contains the <see cref="ActionAttribute.UUID" />.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="actionAnalyzer">The <see cref="ActionAnalyzer"/> that contains the action information.</param>
        static void GenerateClassWithUuidProperty(IndentedTextWriter writer, ActionAnalyzer actionAnalyzer)
        {
            writer.WriteLine($"partial class {actionAnalyzer.Context.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}");
            writer.WriteLine("{");
            writer.Indent++;

            writer.WriteLine("/// <summary>");
            writer.WriteLine("/// Gets the unique identifier of the action as defined by the <see cref=\"StreamDeck.ActionAttribute.UUID\"/>.");
            writer.WriteLine("/// </summary>");
            writer.WriteLine($"public const string {UUID_MEMBER_NAME} = \"{actionAnalyzer.Action.UUID}\";");

            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}
