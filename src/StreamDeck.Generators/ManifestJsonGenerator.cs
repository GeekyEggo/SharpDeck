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
    internal class ManifestJsonGenerator
    {
        /// <summary>
        /// Generates the manifest.json file for the manifest contained within the specified <paramref name="manifestAnalyzer"/>.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="manifestAnalyzer">The <see cref="ManifestAnalyzer"/> that contains the manifest to serialize.</param>
        /// <param name="fileSystem">The <see cref="IFileSystem"/> used to write the file.</param>
        public static void Generate(GeneratorExecutionContext context, ManifestAnalyzer manifestAnalyzer, IFileSystem fileSystem)
        {
            if (!manifestAnalyzer.HasManifest)
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

            fileSystem.WriteAllText(
                path: Path.Combine(projectDirectory, "manifest.json"),
                contents: JsonSerializer.Serialize(manifestAnalyzer.Manifest!),
                encoding: Encoding.UTF8);
        }
    }
}
