namespace StreamDeck.Generators
{
    using System;
    using System.IO;
    using System.Text;
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

            try
            {
                // Determine if the project requires manifest generation.
                if (!context.Compilation.Assembly.TryGetAttribute<ManifestAttribute>(out var manifestAttr))
                {
                    return;
                }

                // Ensure we know where the manifest.json file is located.
                if (!this.TryGetFilePath(context, out var filePath))
                {
                    //context.ReportUnknownProjectDirectory();
                    return;
                }

                if (syntaxReceiver.TryGetActions(context, out var actions))
                {
                    // Construct the manifest, and the actions associated with it
                    var manifest = new Manifest(context.Compilation.Assembly);
                    manifestAttr.Populate(manifest);
                    manifest.Actions.AddRange(actions);
                    manifest.Profiles.AddRange(syntaxReceiver.GetProfiles(context));

                    // Write the manifest file.
                    var json = JsonSerializer.Serialize(manifest);
                    this.FileSystem.WriteAllText(filePath, json, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                //context.ReportException(ex);
            }
        }

        /// <summary>
        /// Attempts to get the manifest.json file path from the additional files defined in the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="filePath">The manifest.json file path.</param>
        /// <returns><c>true</c> when the manifest.json file path was present; otherwise <c>false</c>.</returns>
        private bool TryGetFilePath(GeneratorExecutionContext context, out string filePath)
        {
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDirectory))
            {
                filePath = Path.Combine(projectDirectory, "manifest.json");
                return true;
            }

            filePath = string.Empty;
            return false;
        }
    }
}
