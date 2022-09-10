namespace StreamDeck.Generators
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Models;
    using StreamDeck.Generators.Serialization;

    /// <summary>
    /// Generates the manifest.json file that accompanies a Stream Deck plugin.
    /// </summary>
    internal class ManifestJsonGenerator
    {
        /// <summary>
        /// Generates the manifest.json file for the specified <paramref name="context"/> and <paramref name="actions"/>.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="actions">The actions to write to the manifest.</param>
        /// <param name="fileSystem">The file system.</param>
        public static void Generate(GeneratorExecutionContext context, IReadOnlyCollection<ActionClassDeclarationSyntax> actions, IFileSystem fileSystem)
        {
            // Determine if the assembly requires manifest generation.
            if (!context.Compilation.Assembly.TryGetAttribute<ManifestAttribute>(out var manifestAttr))
            {
                return;
            }

            var diagnostics = new DiagnosticReporter(context);

            // Ensure we know where the manifest.json file will be located.
            if (!context.TryGetProjectDirectory(out var projectDirectory))
            {
                diagnostics.ReportProjectDirectoryNotFoundForManifestJson(context.Compilation.Assembly.Locations);
            }

            // Construct the manifest, and add the actions and profiles associated with it.
            var manifest = new Manifest(context.Compilation.Assembly);
            manifestAttr.Populate(manifest);
            manifest.Actions.AddRange(GetValidActions(actions, diagnostics));
            manifest.Profiles.AddRange(GetProfiles(context));

            // Only write the manifest if everything is okay.
            if (!diagnostics.HasErrorDiagnostic)
            {
                fileSystem.WriteAllText(
                    path: Path.Combine(projectDirectory, "manifest.json"),
                    contents: JsonSerializer.Serialize(manifest),
                    encoding: Encoding.UTF8);
            }
        }

        /// <summary>
        /// Gets the profiles associated with compilation.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <returns>The profiles discovered from the context's compilation assembly.</returns>
        private static IEnumerable<ProfileAttribute> GetProfiles(GeneratorExecutionContext context)
        {
            foreach (var profileAttr in context.Compilation.Assembly.GetAttributes<ProfileAttribute>())
            {
                var item = new ProfileAttribute((string)profileAttr.ConstructorArguments[0].Value!, (Device)profileAttr.ConstructorArguments[1].Value!);
                yield return profileAttr.Populate(item);
            }
        }

        /// <summary>
        /// Gets the valid <see cref="ActionAttribute"/> from the <paramref name="nodes"/>.
        /// </summary>
        /// <param name="nodes">The collection of <see cref="ActionClassDeclarationSyntax"/> nodes discovered by the <see cref="StreamDeckSyntaxReceiver"/>.</param>
        /// <param name="diagnosticReporter">The outer diagnostic reporter.</param>
        /// <returns>The valid actions.</returns>
        private static IEnumerable<ActionAttribute> GetValidActions(IReadOnlyCollection<ActionClassDeclarationSyntax> nodes, DiagnosticReporter diagnosticReporter)
        {
            foreach (var node in nodes)
            {
                var diagnostics = new DiagnosticReporter(diagnosticReporter);

                // Validate UUID characters (https://developer.elgato.com/documentation/stream-deck/sdk/manifest/).
                if (!node.IsUuidValid)
                {
                    diagnostics.ReportInvalidActionUUID(node);
                }

                // Validate the state image is defined.
                if (node.Action.StateImage == null
                    && node.States.Length == 0)
                {
                    diagnostics.ReportStateImageNotDefined(node);
                }

                // Validate the state image is not defined more than once.
                if (node.Action.StateImage != null
                    && node.States.Length > 0)
                {
                    diagnostics.ReportStateImageDefinedMoreThanOnce(node);
                }

                // Validate there are not more than 2 states on the action.
                if (node.States.Length > 2)
                {
                    diagnostics.ReportActionHasTooManyStates(node);
                }

                // When the action did not report an error, add it to the collection of valid actions.
                if (!diagnostics.HasErrorDiagnostic)
                {
                    if (node.States.Length > 0)
                    {
                        node.Action.States = node.States;
                    }

                    yield return node.Action;
                }
            }
        }
    }
}
