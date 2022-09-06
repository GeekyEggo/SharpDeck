namespace StreamDeck.Generators.Tests.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Provides a mock implementation of <see cref="AnalyzerConfigOptionsProvider"/>.
    /// </summary>
    internal class MockAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockAnalyzerConfigOptionsProvider"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MockAnalyzerConfigOptionsProvider(params (string Key, string Value)[] options)
            => this.GlobalOptions = new MockAnalyzerConfigOptions(options);

        /// <inheritdoc/>
        public override AnalyzerConfigOptions GlobalOptions { get; }

        /// <inheritdoc/>
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            => this.GlobalOptions;

        /// <inheritdoc/>
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            => this.GlobalOptions;
    }
}
