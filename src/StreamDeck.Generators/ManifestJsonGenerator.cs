namespace StreamDeck.Generators
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Analyzers;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Serialization;

    /// <summary>
    /// Generates the manifest.json file that accompanies a Stream Deck plugin.
    /// </summary>
    internal class ManifestJsonGenerator : BaseSourceGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestJsonGenerator"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        public ManifestJsonGenerator(IFileSystem fileSystem)
            => this.FileSystem = fileSystem;

        /// <summary>
        /// Gets the file system.
        /// </summary>
        private IFileSystem FileSystem { get; }

        /// <inheritdoc/>
        internal override void Execute(GeneratorExecutionContext context, StreamDeckSyntaxReceiver syntaxReceiver, ManifestAnalyzer manifestAnalyzer)
        {
            if (!manifestAnalyzer.HasManifest
                || manifestAnalyzer.HasErrorDiagnostic)
            {
                return;
            }

            // Ensure we know where the manifest.json file will be located.
            var diagnostics = new DiagnosticReporter(context);
            if (!context.TryGetProjectDirectory(out var projectDirectory))
            {
                diagnostics.ReportProjectDirectoryNotFoundForManifestJson(context.Compilation.Assembly.Locations);
                return;
            }

            this.FileSystem.WriteAllText(
                path: Path.Combine(projectDirectory, "manifest.json"),
                contents: JsonSerializer.Serialize(manifestAnalyzer.Manifest!),
                encoding: Encoding.UTF8);
        }
    }
}
