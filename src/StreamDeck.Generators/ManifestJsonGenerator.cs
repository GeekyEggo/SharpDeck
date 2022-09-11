namespace StreamDeck.Generators
{
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        /// <param name="manifestNode">The <see cref="AttributeSyntax"/> that represents the <see cref="ManifestAttribute"/>.</param>
        /// <param name="actions">The actions to write to the manifest.</param>
        /// <param name="fileSystem">The file system.</param>
        public static void Generate(GeneratorExecutionContext context, AttributeSyntax? manifestNode, IReadOnlyCollection<ActionClassContext> actions, IFileSystem fileSystem)
        {
            // Determine if the assembly requires manifest generation.
            if (manifestNode == null
                || !context.Compilation.Assembly.TryGetAttribute<ManifestAttribute>(out var manifestAttributeData))
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
            var manifest = new Manifest
            {
                Author = context.Compilation.Assembly.GetAttributeValueOrDefault<AssemblyCompanyAttribute, string>() ?? "",
                CodePath = $"{context.Compilation.Assembly.Identity.Name}.exe",
                Description = context.Compilation.Assembly.GetAttributeValueOrDefault<AssemblyDescriptionAttribute, string>() ?? "",
                Name = context.Compilation.Assembly.Identity.Name,
                Version = context.Compilation.Assembly.Identity.Version.ToString(3),
            };

            manifestAttributeData.Populate(manifest);
            manifest.Actions.AddRange(GetValidActions(actions, diagnostics));
            manifest.Profiles.AddRange(GetProfiles(context));

            // Only write the manifest if everything is okay.
            Validate(manifest, manifestNode, diagnostics);
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
        /// <param name="nodes">The collection of <see cref="ActionClassDeclarationSyntax"/> nodes discovered by the <see cref="PluginSyntaxReceiver"/>.</param>
        /// <param name="outerDiagnostics">The outer diagnostic reporter.</param>
        /// <returns>The valid actions.</returns>
        private static IEnumerable<ActionAttribute> GetValidActions(IReadOnlyCollection<ActionClassContext> nodes, DiagnosticReporter outerDiagnostics)
        {
            foreach (var node in nodes)
            {
                var diagnostics = new DiagnosticReporter(outerDiagnostics);

                var action = node.ActionAttribute.As<ActionAttribute>();
                var states = node.StateAttributes.Select(s => s.As<StateAttribute>()).ToArray();

                // Validate UUID characters (https://developer.elgato.com/documentation/stream-deck/sdk/manifest/).
                if (Regex.IsMatch(action.UUID, @"[^a-z0-9\-\.]+"))
                {
                    diagnostics.ReportInvalidActionUUID(node);
                }

                // Validate the state image is defined.
                if (action.StateImage == null
                    && states.Length == 0)
                {
                    diagnostics.ReportStateImageNotDefined(node);
                }

                // Validate the state image is not defined more than once.
                if (action.StateImage != null
                    && states.Length > 0)
                {
                    diagnostics.ReportStateImageDefinedMoreThanOnce(node);
                }

                // Validate there are not more than 2 states on the action.
                if (states.Length > 2)
                {
                    diagnostics.ReportActionHasTooManyStates(node);
                }

                // When the action did not report an error, add it to the collection of valid actions.
                if (!diagnostics.HasErrorDiagnostic)
                {
                    if (states.Length > 0)
                    {
                        action.States = states;
                    }

                    yield return action;
                }
            }
        }

        /// <summary>
        /// Validates the specified <paramref name="manifest"/> and reports all diagnostics to <paramref name="diagnostics"/>.
        /// </summary>
        /// <param name="manifest">The manifest to validate.</param>
        /// <param name="manifestNode">The <see cref="AttributeSyntax"/> that represents the <see cref="ManifestAttribute"/>.</param>
        /// <param name="diagnostics">The diagnostics reporter.</param>
        private static void Validate(Manifest manifest, AttributeSyntax manifestNode, DiagnosticReporter diagnostics)
        {
            if (string.IsNullOrWhiteSpace(manifest.Author))
            {
                manifest.Author = string.Empty; // Ensure a value is always serialized.
                diagnostics.ReportManifestRequiresAuthor(manifestNode);
            }

            if (string.IsNullOrWhiteSpace(manifest.Description))
            {
                manifest.Description = string.Empty; // Ensure a value is always serialized.
                diagnostics.ReportManifestRequiresDescription(manifestNode);
            }

            if (string.IsNullOrWhiteSpace(manifest.Icon))
            {
                manifest.Icon = string.Empty; // Ensure a value is always serialized.
                diagnostics.ReportManifestRequiresIcon(manifestNode);
            }

            if (manifest.Actions.Count == 0)
            {
                diagnostics.ReportManifestRequiresActions(manifestNode);
            }
        }
    }
}
