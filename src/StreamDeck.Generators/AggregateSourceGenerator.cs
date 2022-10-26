namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.CodeAnalysis;
    using StreamDeck.Generators.IO;

    /// <summary>
    /// Provides generation of files relating to the Stream Deck plugin.
    /// </summary>
    [Generator]
    public class AggregateSourceGenerator : BaseSourceGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSourceGenerator"/> class.
        /// </summary>
        public AggregateSourceGenerator()
            : this(new FileSystem()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSourceGenerator"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        internal AggregateSourceGenerator(IFileSystem fileSystem)
            : base()
        {
            this.Generators = new BaseSourceGenerator[]
            {
                new HostExtensionsSourceGenerator(),
                new ManifestJsonGenerator(fileSystem),
                new PropertyInspectorSourceGenerator(fileSystem),
                new UuidPropertySourceGenerator()
            };
        }

        /// <summary>
        /// Gets the generators.
        /// </summary>
        private IReadOnlyCollection<BaseSourceGenerator> Generators { get; }

        /// <inheritdoc/>
        internal override void Execute(GeneratorExecutionContext context, StreamDeckSyntaxReceiver syntaxReceiver, ManifestAnalyzer manifestAnalyzer)
        {
            foreach (var generator in this.Generators)
            {
                generator.Execute(context, syntaxReceiver, manifestAnalyzer);
            }
        }
    }
}
