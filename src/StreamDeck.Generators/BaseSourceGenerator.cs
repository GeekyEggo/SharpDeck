namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Analyzers;

    /// <summary>
    /// Provides a base <see cref="ISourceGenerator"/> that relies on a <see cref="ManifestAnalyzer"/>.
    /// </summary>
    public abstract class BaseSourceGenerator : ISourceGenerator
    {
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

            this.Execute(context, syntaxReceiver, new ManifestAnalyzer(context, syntaxReceiver));
        }

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new StreamDeckSyntaxReceiver());

        /// <summary>
        /// Executes the source generator.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="syntaxReceiver">The <see cref="StreamDeckSyntaxReceiver"/> from the <paramref name="context"/>.</param>
        /// <param name="manifestAnalyzer">The <see cref="ManifestAnalyzer"/>.</param>
        internal abstract void Execute(GeneratorExecutionContext context, StreamDeckSyntaxReceiver syntaxReceiver, ManifestAnalyzer manifestAnalyzer);
    }
}
