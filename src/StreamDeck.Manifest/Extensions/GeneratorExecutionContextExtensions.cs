namespace StreamDeck.Manifest.Extensions
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides extension methods for <see cref="GeneratorExecutionContext"/>.
    /// </summary>
    internal static class GeneratorExecutionContextExtensions
    {
        /// <summary>
        /// The category of all diagnostics reported.
        /// </summary>
        private const string DIAGNOSTIC_CATEGORY = "StreamDeckManifest";

        /// <summary>
        /// The warning level that represents an error.
        /// </summary>
        private const int ERROR_LEVEL = 0;

        /// <summary>
        /// The warning level that represents a warning.
        /// </summary>
        private const int WARNING_LEVEL = 1;

        #region Errors

        /// <summary>
        /// Reports an error due to the manifest file location not being defined.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        internal static void ReportMissingManifestFile(this GeneratorExecutionContext context)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    id: "SDM001",
                    category: DIAGNOSTIC_CATEGORY,
                    message: "Unable to generate manifest: \"manifest.json\" must be referenced as an additional file",
                    severity: DiagnosticSeverity.Error,
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    warningLevel: ERROR_LEVEL));

        /// <summary>
        /// Reports an error due to the compilation assembly missing the <see cref="PluginManifestAttribute"/>.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        internal static void ReportMissingManifestAttribute(this GeneratorExecutionContext context)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    id: "SDM002",
                    category: DIAGNOSTIC_CATEGORY,
                    message: $"Unable to generate manifest: assembly requires {nameof(PluginManifestAttribute)}",
                    severity: DiagnosticSeverity.Error,
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    warningLevel: ERROR_LEVEL));

        /// <summary>
        /// Reports an error due to an action not defining either <see cref="PluginActionAttribute.StateImage"/> or <see cref="PluginActionStateAttribute"/>.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="location">The location of the error.</param>
        internal static void ReportNoActionStatesDefined(this GeneratorExecutionContext context, Location location)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "SDM003",
                        title: "No action states",
                        messageFormat: $"Unable to generate manifest: actions must have either a {nameof(PluginActionAttribute)}.{nameof(PluginActionAttribute.StateImage)}, or up to two {nameof(PluginActionStateAttribute)}",
                        category: DIAGNOSTIC_CATEGORY,
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    location));

        /// <summary>
        /// Generates a new diagnostic used to indicate when an action has more than two states.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="location">The location of the error.</param>
        internal static void ReportTooManyActionStates(this GeneratorExecutionContext context, Location location)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "SDM004",
                        title: "Too many action states",
                        messageFormat: $"Unable to generate manifest: actions can have a maximum of two states",
                        category: DIAGNOSTIC_CATEGORY,
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    location));

        #endregion

        #region Warnings

        /// <summary>
        /// Reports a warning due to the existence of <see cref="PluginActionAttribute.StateImage"/> when a <see cref="PluginActionStateAttribute"/> is defined.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="location">The location of the warning.</param>
        internal static void ReportStateImageValueObsolete(this GeneratorExecutionContext context, Location location)
            => Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "SDM101",
                    title: $"Unnecessary \"{nameof(PluginActionAttribute.StateImage)}\"",
                    messageFormat: $"Unnecessary \"{nameof(PluginActionAttribute.StateImage)}\" on action: states are already defined by {nameof(PluginActionStateAttribute)}",
                    category: DIAGNOSTIC_CATEGORY,
                    defaultSeverity: DiagnosticSeverity.Warning,
                    isEnabledByDefault: true),
                location);

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

        #endregion
    }
}
