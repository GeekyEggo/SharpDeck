namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Analyzers;
    using StreamDeck.Generators.IO;

    /// <summary>
    /// Provides generation of files relating to the Stream Deck plugin.
    /// </summary>
    [Generator]
    public class PluginSourceGenerator : ISourceGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginSourceGenerator"/> class.
        /// </summary>
        public PluginSourceGenerator()
            : this(new FileSystem()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginSourceGenerator"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        internal PluginSourceGenerator(IFileSystem fileSystem)
            => this.FileSystem = fileSystem;

        /// <summary>
        /// Gets the file system responsible for handling files.
        /// </summary>
        private IFileSystem FileSystem { get; }

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new PluginSyntaxReceiver());

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // Should only generate the manifest file if we have a receiver.
            if (context.SyntaxContextReceiver is not PluginSyntaxReceiver syntaxReceiver
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

            var manifestAnalyzer = new ManifestAnalyzer(context, syntaxReceiver);

            ManifestJsonGenerator.Generate(context, manifestAnalyzer, this.FileSystem);
            UuidPropertySourceGenerator.Generate(context, manifestAnalyzer);
            HostExtensionsSourceGenerator.Generate(context, manifestAnalyzer);
        }
    }
}
