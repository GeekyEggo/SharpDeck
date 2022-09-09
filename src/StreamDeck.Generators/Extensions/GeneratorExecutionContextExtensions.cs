namespace StreamDeck.Generators.Extensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides extension methods for <see cref="GeneratorExecutionContext"/>.
    /// </summary>
    internal static class GeneratorExecutionContextExtensions
    {
        /// <summary>
        /// The category of all diagnostics reported.
        /// </summary>
        private const string DIAGNOSTIC_CATEGORY = "StreamDeck";

        /// <summary>
        /// Reports a <see cref="Diagnostic"/> with a severity of <see cref="DiagnosticSeverity.Error"/> error.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/> where the <see cref="Diagnostic"/> is being reported.</param>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="locations">The optional <see cref="Diagnostic.Location"/>; when more than one, the first is taken.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        internal static void ReportError(this GeneratorExecutionContext context, string id, string title, string messageFormat, IEnumerable<Location>? locations = null, params string[] messageArgs)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: id,
                        title: title,
                        messageFormat: messageFormat,
                        category: DIAGNOSTIC_CATEGORY,
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    locations?.FirstOrDefault(),
                    messageArgs));

        /// <summary>
        /// Reports a <see cref="Diagnostic"/> with a severity of <see cref="DiagnosticSeverity.Warning"/> error.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/> where the <see cref="Diagnostic"/> is being reported.</param>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="locations">The optional <see cref="Diagnostic.Location"/>; when more than one, the first is taken.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        internal static void ReportWarning(this GeneratorExecutionContext context, string id, string title, string messageFormat, IEnumerable<Location>? locations = null, params string[] messageArgs)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: id,
                        title: title,
                        messageFormat: messageFormat,
                        category: DIAGNOSTIC_CATEGORY,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true),
                    locations?.FirstOrDefault(),
                    messageArgs));

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
