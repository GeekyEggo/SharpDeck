namespace StreamDeck.Generators.Tests.Helpers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides assertions for <see cref="Diagnostic"/>.
    /// </summary>
    internal static class DiagnosticAssert
    {
        /// <summary>
        /// Asserts two collections of <see cref="Diagnostic"/> are equal.
        /// </summary>
        /// <param name="actual">The actual collection of <see cref="Diagnostic"/>.</param>
        /// <param name="expected">The expected collection of <see cref="Diagnostic"/>.</param>
        internal static void AreEqual(ImmutableArray<Diagnostic> actual, IReadOnlyList<ExpectedDiagnostic> expected)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count));
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(actual[i].ToString(), Is.EqualTo(expected[i].ToString()));
            }
        }
    }

    /// <summary>
    /// Provides information about an expected diagnostic.
    /// </summary>
    internal record struct ExpectedDiagnostic(int Row, int Column, string Id, string Description, DiagnosticSeverity Severity)
    {
        /// <inheritdoc/>
        public override string ToString()
            => $"({this.Row},{this.Column}): {this.Severity.ToString().ToLower()} {this.Id}: {this.Description}";
    }
}
