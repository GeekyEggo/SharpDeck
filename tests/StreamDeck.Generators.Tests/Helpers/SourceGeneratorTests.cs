namespace StreamDeck.Generators.Tests.Helpers
{
    using System.Collections.Immutable;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StreamDeck.Generators.IO;

    /// <summary>
    /// Provides a base class responsible for testing a <see cref="ISourceGenerator"/>.
    /// </summary>
    internal class SourceGeneratorTests
    {
        /// <summary>
        /// The default assembly name.
        /// </summary>
        internal const string ASSEMBLY_NAME = "TestProject";

        /// <summary>
        /// The default project directory.
        /// </summary>
        internal const string PROJECT_DIRECTORY = @"C:\temp\";

        /// <summary>
        /// Gets the default <see cref="AnalyzerConfigOptionsProvider"/>.
        /// </summary>
        internal static AnalyzerConfigOptionsProvider DEFAULT_OPTIONS_PROVIDER => new MockAnalyzerConfigOptionsProvider(("build_property.projectdir", PROJECT_DIRECTORY));

        /// <summary>
        /// Runs the <paramref name="sourceText"/> as CSharp against the provided <paramref name="generator"/>.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="sourceText">The source text.</param>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="optionsProvider">The optional <see cref="AnalyzerConfigOptionsProvider"/>.</param>
        /// <returns>The output compilation and diagnostics reported during execution of the <see cref="ISourceGenerator"/>.</returns>
        internal static (Compilation? OutputCompilation, ImmutableArray<Diagnostic> Diagnostics) Run(
            ISourceGenerator generator,
            string sourceText,
            string? assemblyName = ASSEMBLY_NAME,
            AnalyzerConfigOptionsProvider? optionsProvider = null)
        {
            // Parse the provided source text into a C# syntax tree.
            var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(sourceText) };

            // Generate the references (credit https://github.com/andrewlock/StronglyTypedId/blob/6d36325be98e90779bd6bac6c9b99a6015fcec7d/test/StronglyTypedIds.Tests/TestHelpers.cs#L17).
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location) && a != typeof(AggregateSourceGenerator).Assembly)
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .Concat(new[]
                {
                    MetadataReference.CreateFromFile(typeof(FontFamily).Assembly.Location), // StreamDeck.Abstractions
                });

            // Create a Roslyn compilation for the syntax tree and references; we can always assert against a console application.
            var compilation = CSharpCompilation.Create(
                assemblyName ?? ASSEMBLY_NAME,
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

        /// <summary>
        /// Verifies <see cref="IFileSystem.WriteAllText(string, string, Encoding)"/> was invoked once per <paramref name="sources"/>.
        /// </summary>
        /// <param name="fileSystem">The file system to verify against.</param>
        /// <param name="sources">The expected sources.</param>
        internal static void VerifyFiles(Mock<IFileSystem> fileSystem, params (string HintName, string SourceText)[] sources)
        {
            if (sources.Length == 0)
            {
                fileSystem.Verify(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Encoding>()), Times.Never);
            }
            else
            {
                foreach (var (HintName, SourceText) in sources)
                {
                    fileSystem.Verify(f => f.WriteAllText($"{PROJECT_DIRECTORY}{HintName}", SourceText, Encoding.UTF8), Times.Once);
                }
            }
        }

        /// <summary>
        /// Verifies <see cref="Compilation.SyntaxTrees"/> as generated files against the expected <paramref name="sources"/>.
        /// </summary>
        /// <param name="compilation">The compilation containing the syntax trees.</param>
        /// <param name="sources">The expected sources.</param>
        internal static void VerifySources(Compilation? compilation, params (string HintName, string SourceText)[] sources)
        {
            Assert.That(compilation, Is.Not.Null);

            var actualSources = compilation.SyntaxTrees.Skip(1).ToArray();
            Assert.That(actualSources, Has.Length.EqualTo(sources.Length));

            for (var i = 0; i < sources.Length; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(actualSources[i].FilePath, Is.EqualTo(sources[i].HintName), $"FilePath of syntax trees differs");
                    Assert.That(actualSources[i].ToString(), Is.EqualTo(sources[i].SourceText), $"SourceText of syntax tree differs");
                });
            }
        }
    }
}
