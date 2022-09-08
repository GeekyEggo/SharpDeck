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

        #endregion

        #region Actions

        /// <summary>
        /// Reports a <see cref="Diagnostic"/> on the <see cref="GeneratorExecutionContext"/>, indicating the <see cref="ActionAttribute.StateImage"/> cannot be defined when a <see cref="StateAttribute"/> is present.
        /// </summary>
        /// <param name="context">The <see cref="GeneratorExecutionContext"/>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="location">The location of the error.</param>
        internal static void ReportStateImageDefinedMoreThanOnce(this GeneratorExecutionContext context, string actionName, Location? location)
            => context.ReportError(
                "SD101",
                "State images should not be defined more than once",
                $"Action, '{{0}}', should not set the '{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}' when a '{nameof(StateAttribute)}' is present.",
                location,
                actionName);

        #endregion

        #region Errors
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
        /// Reports an error due to an action not defining either <see cref="ActionAttribute.StateImage"/> or <see cref="StateAttribute"/>.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="location">The location of the error.</param>
        internal static void ReportNoActionStatesDefined(this GeneratorExecutionContext context, Location location)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "SDM002",
                        title: "No action states",
                        messageFormat: $"Unable to generate manifest: actions must have either a {nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}, or up to two {nameof(StateAttribute)}",
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
                        id: "SDM003",
                        title: "Too many action states",
                        messageFormat: "Unable to generate manifest: actions can have a maximum of two states",
                        category: DIAGNOSTIC_CATEGORY,
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    location));

        /// <summary>
        /// Generates a new diagnostic used to indicate when an action's UUID contains invalid characters.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="location">The location of the error.</param>
        internal static void ReportInvalidUUIDCharacters(this GeneratorExecutionContext context, Location location)
            => context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "SDM004",
                        title: "Invalid UUID",
                        messageFormat: "The action's UUID can only contain lowercase alphanumeric characters (a-z, 0-9), hyphen (-), and period (.)",
                        category: DIAGNOSTIC_CATEGORY,
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    location));

        #endregion

        #region Warnings

        /// <summary>
        /// Reports a warning due to the existence of <see cref="ActionAttribute.StateImage"/> when a <see cref="StateAttribute"/> is defined.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="location">The location of the warning.</param>
        internal static void ReportStateImageValueObsolete(this GeneratorExecutionContext context, Location location)
            => Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "SDM101",
                    title: $"Unnecessary \"{nameof(ActionAttribute.StateImage)}\"",
                    messageFormat: $"Unnecessary \"{nameof(ActionAttribute.StateImage)}\" on action: states are already defined by {nameof(StateAttribute)}",
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
