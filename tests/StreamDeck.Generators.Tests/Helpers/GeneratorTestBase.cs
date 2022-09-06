namespace StreamDeck.Generators.Tests.Helpers
{
    using System.Collections.Immutable;
    using System.Reflection;
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
        /// Runs the generator.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="globalOptions">The global options.</param>
        /// <returns>The updated compilation, after running the source generator.</returns>
        protected static Compilation RunGenerator(string sourceText, ISourceGenerator generator, params (string Key, string Value)[] globalOptions)
            => RunGenerator(sourceText, generator, out _, globalOptions);

        /// <summary>
        /// Runs the generator.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="diagnostics">The diagnostics.</param>
        /// <param name="globalOptions">The global options.</param>
        /// <returns>The updated compilation, after running the source generator.</returns>
        protected static Compilation RunGenerator(string sourceText, ISourceGenerator generator, out ImmutableArray<Diagnostic> diagnostics, params (string Key, string Value)[] globalOptions)
        {
            var compilation = CreateCompilation(sourceText);
            var driver = CSharpGeneratorDriver.Create(
                generators: ImmutableArray.Create(generator),
                additionalTexts: ImmutableArray<AdditionalText>.Empty,
                parseOptions: (CSharpParseOptions)compilation.SyntaxTrees.First().Options,
                optionsProvider: new MockAnalyzerConfigOptionsProvider(globalOptions));

            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out diagnostics);
            return updatedCompilation;
        }

        /// <summary>
        /// Creates a compilation from the specified <paramref name="sourceText"/>.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="assemblyName">Name of the assembly; otherwise <see cref="DEFAULT_ASSEMBLY_NAME"/>.</param>
        /// <returns>The compilation.</returns>
        protected static Compilation CreateCompilation(string sourceText, string assemblyName = DEFAULT_ASSEMBLY_NAME)
            => CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(sourceText, new CSharpParseOptions(LanguageVersion.Preview)) },
                references: new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}
