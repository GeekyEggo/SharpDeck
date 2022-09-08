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

        #region Manifest

        /*
        /// <summary>
        /// Reports an error due to not being able to determine the project's directory.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        internal static void ReportUnknownProjectDirectory(this GeneratorExecutionContext context)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    id: "SDM001",
                    category: DIAGNOSTIC_CATEGORY,
                    message: "Unable to generate manifest: the project's directory could not be determined",
                    severity: DiagnosticSeverity.Error,
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    warningLevel: ERROR_LEVEL));

        /// <summary>
        /// Reports an error due to an exception encountered in the generator.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="ex">The exception.</param>
        internal static void ReportException(this GeneratorExecutionContext context, Exception ex)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    id: "SDM113",
                    category: DIAGNOSTIC_CATEGORY,
                    message: $"Failed to generate manifest: {ex.Message}",
                    severity: DiagnosticSeverity.Warning, // Warn, so we don't block the consuming application
                    defaultSeverity: DiagnosticSeverity.Warning,
                    isEnabledByDefault: true,
                    warningLevel: WARNING_LEVEL));
        */

        #endregion

        /// <summary>
        /// Reports a <see cref="Diagnostic"/> with a severity of <see cref="DiagnosticSeverity.Error"/> error.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/> where the <see cref="Diagnostic"/> is being reported.</param>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="location">The optional <see cref="Diagnostic.Location"/>.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        internal static void ReportError(this GeneratorExecutionContext context, string id, string title, string messageFormat, Location? location, params string[] messageArgs)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: id,
                        title: title,
                        messageFormat: messageFormat,
                        category: DIAGNOSTIC_CATEGORY,
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    location,
                    messageArgs));

        /// <summary>
        /// Reports a <see cref="Diagnostic"/> with a severity of <see cref="DiagnosticSeverity.Warning"/> error.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/> where the <see cref="Diagnostic"/> is being reported.</param>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="location">The optional <see cref="Diagnostic.Location"/>.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        internal static void ReportWarning(this GeneratorExecutionContext context, string id, string title, string messageFormat, Location? location, params string[] messageArgs)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: id,
                        title: title,
                        messageFormat: messageFormat,
                        category: DIAGNOSTIC_CATEGORY,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true),
                    location,
                    messageArgs));
    }
}
