namespace StreamDeck.Generators
{
    using System;
    using System.CodeDom.Compiler;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Models;
    using StreamDeck.Generators.Serialization;

    /// <summary>
    /// Provides auto-generation of the manifest.json file that accompanies a Stream Deck plugin.
    /// </summary>
    [Generator]
    public class ManifestSourceGenerator : ISourceGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestSourceGenerator"/> class.
        /// </summary>
        public ManifestSourceGenerator()
            : this(new FileSystem()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestSourceGenerator"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        internal ManifestSourceGenerator(IFileSystem fileSystem)
            => this.FileSystem = fileSystem;

        /// <summary>
        /// Gets the file system responsible for handling files.
        /// </summary>
        private IFileSystem FileSystem { get; }

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new ManifestSyntaxReceiver());

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // Should only generate the manifest file if we have a receiver.
            if (context.SyntaxContextReceiver is not ManifestSyntaxReceiver syntaxReceiver
                || context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

#if DEBUG_GENERATOR
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif

            // Determine if the assembly requires manifest generation.
            if (!context.Compilation.Assembly.TryGetAttribute<ManifestAttribute>(out var manifestAttr))
            {
                return;
            }

            // Ensure we know where the manifest.json file will be located.
            if (!context.TryGetProjectDirectory(out var projectDirectory))
            {
                context.ReportError(
                    "SD001",
                    "Generating a manifest.json requires a project directory",
                    "Failed to generate manifest JSON file; unable to determine the project's directory from the compilation context.",
                    context.Compilation.Assembly.Locations);

                return;
            }

            // Ensure all actions are valid.
            if (!TryGetActions(context, syntaxReceiver.ActionNodes, out var actions))
            {
                return;
            }

            try
            {
                // Construct the manifest, and the actions associated with it
                var manifest = new Manifest(context.Compilation.Assembly);
                manifestAttr.Populate(manifest);
                manifest.Actions.AddRange(actions);
                manifest.Profiles.AddRange(GetProfiles(context));

                // Write the manifest file.
                this.FileSystem.WriteAllText(
                    path: Path.Combine(projectDirectory, "manifest.json"),
                    contents: JsonSerializer.Serialize(manifest),
                    encoding: Encoding.UTF8);

            }
            catch (Exception ex)
            {
                context.ReportError("SD013", "Unexpected exception", "Failed to generate manifest JSON file; encountered '{0}'.", messageArgs: ex.GetType().Name);
            }
        }

        /// <summary>
        /// Gets the profiles associated with compilation.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
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
        /// Gets the <see cref="ActionAttribute"/> from the <paramref name="nodes"/>, and returns <c>true</c> when all actions pass validation.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="nodes">The collection of <see cref="ActionClassDeclarationSyntax"/> nodes discovered by the <see cref="ManifestSyntaxReceiver"/>.</param>
        /// <param name="actions">The collection of parsed <see cref="ActionAttribute"/>.</param>
        /// <returns><c>true</c> when all actions are valid; otherwise <c>false</c>.</returns>
        private static bool TryGetActions(GeneratorExecutionContext context, IReadOnlyCollection<ActionClassDeclarationSyntax> nodes, out List<ActionAttribute> actions)
        {
            var success = true;
            actions = new List<ActionAttribute>();

            foreach (var node in nodes)
            {
                var isActionValid = true;

                // Validate UUID characters (https://developer.elgato.com/documentation/stream-deck/sdk/manifest/).
                if (Regex.IsMatch(node.Action.UUID, @"[^a-z0-9\-\.]+"))
                {
                    isActionValid = false;
                    context.ReportError(
                        "SD101",
                        "Action identifiers must be valid uniform type identifiers (UTI)",
                        $"Action '{{0}}' must have a valid UUID; identifiers can only contain lowercase alphanumeric characters (a-z, 0-9), hyphens (-), and periods (.).",
                        node.Locations,
                        node.Action.Name);
                }

                // Validate the state image is defined.
                if (node.Action.StateImage == null
                    && node.States.Length == 0)
                {
                    isActionValid = false;
                    context.ReportError(
                        "SD102",
                        "State image must be defined",
                        $"Action '{{0}}' must have a state image; set the '{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}', or add a '{nameof(StateAttribute)}'.",
                        node.Locations,
                        node.Action.Name);
                }

                // Validate the state image is not defined more than once.
                if (node.Action.StateImage != null
                    && node.States.Length > 0)
                {
                    isActionValid = false;
                    context.ReportError(
                        "SD103",
                        "State must not be defined more than once",
                        $"Action '{{0}}' must not set the '{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}' when a '{nameof(StateAttribute)}' is present.",
                        node.Locations,
                        node.Action.Name);
                }

                // Validate there are not more than 2 states on the action.
                if (node.States.Length > 2)
                {
                    isActionValid = false;
                    context.ReportError(
                        "SD104",
                        "Actions cannot have more than two states",
                        $"Action '{{0}}' cannot have more than two states ('{nameof(StateAttribute)}').",
                        node.Locations,
                        node.Action.Name);
                }

                if (node.States.Length > 0)
                {
                    node.Action.States = node.States;
                }

                if (isActionValid)
                {
                    GenerateUUIDOnClass(context, node.Symbol, node.Action.UUID);
                    actions.Add(node.Action);
                }

                success = isActionValid;
            }

            return success;
        }

        /// <summary>
        /// Adds the UUID to class.
        /// </summary>
        /// <returns></returns>
        private static void GenerateUUIDOnClass(GeneratorExecutionContext context, ISymbol classSymbol, string uuid)
        {
            using var writer = new IndentedTextWriter(new StringWriter());

            writer.WriteLine("// <auto-generated />");

            // When we aren't on the global namespace, we should scope the class to the specified namespace
            if (!classSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                writer.WriteLine($"namespace {classSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace)}");
                writer.WriteLine("{");
                writer.Indent++;

                GenerateClass();

                writer.Indent--;
                writer.WriteLine("}");
            }
            else
            {
                GenerateClass();
            }

            // Add the source.
            context.AddSource(
                hintName: string.IsNullOrWhiteSpace(classSymbol.Name) ? $"{uuid}.g" : $"{classSymbol.Name}.g",
                writer.InnerWriter.ToString());

            // Generates the class, and the UUID property.
            void GenerateClass()
            {
                writer.WriteLine($"partial class {classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}");
                writer.WriteLine("{");
                writer.Indent++;

                writer.WriteLine("/// <summary>");
                writer.WriteLine("/// Gets the unique identifier of the action as defined by the <see cref=\"StreamDeck.ActionAttribute.UUID\"/>.");
                writer.WriteLine("/// </summary>");
                writer.WriteLine($"public const string UUID = \"{uuid}\";");

                writer.Indent--;
                writer.WriteLine("}");
            }
        }
    }
}
