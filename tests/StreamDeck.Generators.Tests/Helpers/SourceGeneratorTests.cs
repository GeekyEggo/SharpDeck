namespace StreamDeck.Generators.Tests.Helpers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Provides a base class responsible for testing a <see cref="ISourceGenerator"/>.
    /// </summary>
    internal class SourceGeneratorTests
    {
        /// <summary>
        /// The default assembly name.
        /// </summary>
        internal const string DEFAULT_ASSEMBLY_NAME = "TestProject";

        /// <summary>
        /// Gets the default <see cref="AnalyzerConfigOptionsProvider"/>.
        /// </summary>
        internal static AnalyzerConfigOptionsProvider DEFAULT_OPTIONS_PROVIDER => new MockAnalyzerConfigOptionsProvider(("build_property.projectdir", "C:\\temp\\"));

        /// <summary>
        /// Runs the <paramref name="sourceText"/> as CSharp against the provided <paramref name="generator"/>.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="sourceText">The source text.</param>
        /// <param name="optionsProvider">The optional <see cref="AnalyzerConfigOptionsProvider"/>.</param>
        /// <returns>The output compilation and diagnostics reported during execution of the <see cref="ISourceGenerator"/>.</returns>
        internal static (Compilation? OutputCompilation, ImmutableArray<Diagnostic> Diagnostics) Run(ISourceGenerator generator, string sourceText, AnalyzerConfigOptionsProvider? optionsProvider = null)
        {
            // Parse the provided source text into a C# syntax tree.
            var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(sourceText) };

            // Generate the references (credit https://github.com/andrewlock/StronglyTypedId/blob/6d36325be98e90779bd6bac6c9b99a6015fcec7d/test/StronglyTypedIds.Tests/TestHelpers.cs#L17).
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location) && a != typeof(PluginSourceGenerator).Assembly)
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .Concat(new[]
                {
                    MetadataReference.CreateFromFile(typeof(FontFamily).Assembly.Location), // StreamDeck.Abstractions
                });

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
                optionsProvider: optionsProvider ?? DEFAULT_OPTIONS_PROVIDER);

            // Run the driver, and return the diagnostics.
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
            return (outputCompilation, diagnostics);
        }
    }
}
