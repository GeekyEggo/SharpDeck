namespace StreamDeck.Generators
{
    using System.CodeDom.Compiler;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Models;

    /// <summary>
    /// Generates the const UUID property for classes with <see cref="ActionAttribute"/>.
    /// </summary>
    internal class UuidPropertySourceGenerator
    {
        /// <summary>
        /// Generates the const UUID property for all <paramref name="actions" />.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="actions">The actions.</param>
        public static void Generate(GeneratorExecutionContext context, IReadOnlyCollection<ActionClassDeclarationSyntax> actions)
        {
            var hintNameIndexes = new Dictionary<string, int>();

            foreach (var node in actions)
            {
                using var writer = new IndentedTextWriter(new StringWriter());

                writer.WriteLine("// <auto-generated />");

                // When we aren't on the global namespace, we should scope the class to the specified namespace
                if (!node.Symbol.ContainingNamespace.IsGlobalNamespace)
                {
                    writer.WriteLine($"namespace {node.Symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace)}");
                    writer.WriteLine("{");
                    writer.Indent++;

                    GenerateClassWithUuidProperty(writer, node);

                    writer.Indent--;
                    writer.WriteLine("}");
                }
                else
                {
                    GenerateClassWithUuidProperty(writer, node);
                }

                hintNameIndexes[node.Action.UUID] = hintNameIndexes.TryGetValue(node.Action.UUID, out var index) ? ++index : 0;

                // Add the source.
                context.AddSource(
                    hintName: $"{node.Action.UUID}.{index}.g",
                    writer.InnerWriter.ToString());
            }
        }

        /// <summary>
        /// Generates the partial class that contains the <see cref="ActionAttribute.UUID"/>.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="node">The node containing the class action information.</param>
        static void GenerateClassWithUuidProperty(IndentedTextWriter writer, ActionClassDeclarationSyntax node)
        {
            writer.WriteLine($"partial class {node.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}");
            writer.WriteLine("{");
            writer.Indent++;

            writer.WriteLine("/// <summary>");
            writer.WriteLine("/// Gets the unique identifier of the action as defined by the <see cref=\"StreamDeck.ActionAttribute.UUID\"/>.");
            writer.WriteLine("/// </summary>");
            writer.WriteLine($"public const string UUID = \"{node.Action.UUID}\";");

            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}
