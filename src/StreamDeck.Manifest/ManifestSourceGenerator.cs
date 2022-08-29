namespace StreamDeck.Manifest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Manifest.Extensions;
    using StreamDeck.Manifest.Serialization;

    /// <summary>
    /// Provides auto-generation of the manifest.json file that accompanies a Stream Deck plugin.
    /// </summary>
    [Generator]
    public class ManifestSourceGenerator : ISourceGenerator
    {
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

            try
            {
                // Ensure we know where the manifest.json file is located.
                if (!this.TryGetFilePath(context, out var filePath))
                {
                    context.ReportMissingManifestFile();
                    return;
                }

                // Ensure we have the manifest attribute associated with the assembly.
                if (!context.Compilation.Assembly.TryGetAttribute<PluginManifestAttribute>(out var manifestAttr))
                {
                    context.ReportMissingManifestAttribute();
                    return;
                }

                // Construct the manifest, and the actions associated with it
                var manifest = new PluginManifestAttribute(context.Compilation.Assembly);
                manifestAttr.Populate(manifest);
                manifest.Actions.AddRange(syntaxReceiver.GetActions(context));

                // Write the manifest file.
                var json = JsonSerializer.Serialize(manifest);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                context.ReportException(ex);
            }
        }

        /// <summary>
        /// Attempts to get the manifest.json file path from the additional files defined in the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="filePath">The manifest.json file path.</param>
        /// <returns><c>true</c> when the manifest.json file path was present; otherwise <c>false</c>.</returns>
        internal bool TryGetFilePath(GeneratorExecutionContext context, out string filePath)
        {
            filePath = context.AdditionalFiles.FirstOrDefault(a => a.Path.EndsWith("manifest.json", StringComparison.OrdinalIgnoreCase))?.Path ?? string.Empty;
            return !string.IsNullOrEmpty(filePath);
        }
    }
}
