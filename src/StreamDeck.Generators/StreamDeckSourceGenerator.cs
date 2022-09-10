namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.IO;

    /// <summary>
    /// Provides generation of files relating to the Stream Deck plugin.
    /// </summary>
    [Generator]
    public class StreamDeckSourceGenerator : ISourceGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckSourceGenerator"/> class.
        /// </summary>
        public StreamDeckSourceGenerator()
            : this(new FileSystem()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckSourceGenerator"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        internal StreamDeckSourceGenerator(IFileSystem fileSystem)
            => this.FileSystem = fileSystem;

        /// <summary>
        /// Gets the file system responsible for handling files.
        /// </summary>
        private IFileSystem FileSystem { get; }

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new StreamDeckSyntaxReceiver());

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // Should only generate the manifest file if we have a receiver.
            if (context.SyntaxContextReceiver is not StreamDeckSyntaxReceiver syntaxReceiver
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

            ManifestJsonGenerator.Generate(context, syntaxReceiver.ActionNodes, this.FileSystem);
            UuidPropertySourceGenerator.Generate(context, syntaxReceiver.ActionNodes);
            HostExtensionsSourceGenerator.Generate(context, syntaxReceiver.ActionNodes);
        }
    }
}
