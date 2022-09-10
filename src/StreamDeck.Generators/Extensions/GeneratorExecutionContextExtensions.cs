namespace StreamDeck.Generators.Extensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides extension methods for <see cref="GeneratorExecutionContext"/>.
    /// </summary>
    internal static class GeneratorExecutionContextExtensions
    {
        /// <summary>
        /// Attempts to get the project directory associated with the <see cref="GeneratorExecutionContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="path">The project directory path.</param>
        /// <returns><c>true</c> when the project directory was present within the <see cref="GeneratorExecutionContext"/> global options; otherwise <c>false</c>.</returns>
        internal static bool TryGetProjectDirectory(this GeneratorExecutionContext context, out string? path)
            => context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out path);
    }
}
