namespace StreamDeck.Generators.Tests.Helpers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Provides a base class responsible for testing a <see cref="ISourceGenerator"/>.
    /// </summary>
    public class GeneratorTestBase
    {
        /// <summary>
        /// The default assembly name.
        /// </summary>
        public const string DEFAULT_ASSEMBLY_NAME = "TestProject";

        /// <summary>
        /// Runs the <paramref name="sourceText"/> as CSharp against the provided <paramref name="generator"/>.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="sourceText">The source text.</param>
        /// <param name="globalOptions">The global options.</param>
        /// <returns>The diagnostics reported during execution of the <see cref="ISourceGenerator"/>.</returns>
        protected static ImmutableArray<Diagnostic> RunGenerator(ISourceGenerator generator, string sourceText, params (string Key, string Value)[] globalOptions)
        {
            // Parse the provided source text into a C# syntax tree
            var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(sourceText) };
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ManifestAttribute).Assembly.Location)
            };

            // Create a Roslyn compilation for the syntax tree and references; we can always assert against a console application.
            var compilation = CSharpCompilation.Create(
                DEFAULT_ASSEMBLY_NAME,
                syntaxTrees,
                references,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            // Create the driver capable of running the generator.
            var driver = CSharpGeneratorDriver.Create(
                generators: ImmutableArray.Create(generator),
                parseOptions: (CSharpParseOptions)syntaxTrees[0].Options,
                optionsProvider: new MockAnalyzerConfigOptionsProvider(globalOptions));

            // Run the driver, and return the diagnostics.
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var _, out var diagnostics);
            return diagnostics;
        }
    }
}
