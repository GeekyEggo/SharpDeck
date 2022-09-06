namespace StreamDeck.Generators.Tests.Helpers
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Provides a mock implementation of <see cref="AnalyzerConfigOptions"/>.
    /// </summary>
    internal class MockAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockAnalyzerConfigOptions"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MockAnalyzerConfigOptions(params (string Key, string Value)[] options)
            => this.Options = options.ToDictionary(x => x.Key, x => x.Value);

        /// <inheritdoc/>
        public IDictionary<string, string> Options { get; } = new Dictionary<string, string>();

        /// <inheritdoc/>
        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
            => this.Options.TryGetValue(key, out value);
    }
}
