namespace SharpDeck.Manifest
{
    using McMaster.Extensions.CommandLineUtils;

    /// <summary>
    /// Provides conversion for arguments supplied to the manifest writer, typically supplied by a CLI.
    /// </summary>
    internal class ManifestWriterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestWriterParameters"/> class.
        /// </summary>
        /// <param name="app">The application.</param>
        public ManifestWriterParameters(CommandLineApplication app)
        {
            this.WriteManifest = app.Option("-m | --manifest", "Flag indicating the manifest should be compiled", CommandOptionType.NoValue);
            this.AssemblyPath = app.Option("-a | --assembly", "The assembly of the plugin", CommandOptionType.SingleValue);
            this.OutputPath = app.Option("-o | --output", "The output path of the manifest file", CommandOptionType.SingleValue);
            app.HelpOption("-? | -h | --help");
        }

        /// <summary>
        /// Gets the command option that indicates as to whether the manifest should be written.
        /// </summary>
        public CommandOption WriteManifest { get; }

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        public CommandOption AssemblyPath { get; }

        /// <summary>
        /// Gets the output path, where the manifest file should be saved.
        /// </summary>
        public CommandOption OutputPath { get; }
    }
}
