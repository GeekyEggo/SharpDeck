namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides methods for reporting and monitoring diagnostics reported to a <see cref="GeneratorExecutionContext"/>.
    /// </summary>
    internal class DiagnosticReporter
    {
        /// <summary>
        /// The category of all diagnostics reported.
        /// </summary>
        private const string DIAGNOSTIC_CATEGORY = "StreamDeck";

        /// <summary>
        /// Private member field for <see cref="HasErrorDiagnostic"/>.
        /// </summary>
        private bool _hasErrorDiagnostic = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticReporter"/> struct.
        /// </summary>
        /// <param name="context">The context.</param>
        public DiagnosticReporter(GeneratorExecutionContext context)
            => this.Context = context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticReporter"/> struct.
        /// </summary>
        /// <param name="innerReporter">The inner reporter; when an error is reported, it is propagated to this reporter.</param>
        public DiagnosticReporter(DiagnosticReporter innerReporter)
        {
            this.Context = innerReporter.Context;
            this.InnerReporter = innerReporter;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has reported a diagnostic of type <see cref="DiagnosticSeverity.Error"/>.
        /// </summary>
        public bool HasErrorDiagnostic
        {
            get => this._hasErrorDiagnostic;
            private set
            {
                this._hasErrorDiagnostic = value;
                if (this.InnerReporter is not null)
                {
                    this.InnerReporter.HasErrorDiagnostic = value;
                }
            }
        }

        /// <summary>
        /// Gets the inner <see cref="DiagnosticReporter"/>.
        /// </summary>
        private DiagnosticReporter? InnerReporter { get; }

        /// <summary>
        /// Gets the <see cref="GeneratorExecutionContext"/>.
        /// </summary>
        private GeneratorExecutionContext Context { get; }

        /// <summary>
        /// Reports the project directory could not be found when attempting to generate the manifest.json file.
        /// </summary>
        /// <param name="locations">The locations associated with the assembly that attempted to generate the manifest.json file..</param>
        public void ReportProjectDirectoryNotFoundForManifestJson(IEnumerable<Location> locations)
            => this.ReportError(
                "SD001",
                "Generating a manifest.json requires a project directory",
                "Failed to generate manifest JSON file; unable to determine the project's directory from the compilation context.",
                locations);

        /// <summary>
        /// Reports the <see cref="ActionAttribute.UUID"/> is invalid.
        /// </summary>
        /// <param name="context">The context containing information about the class declaration of the action.</param>
        public void ReportInvalidActionUUID(ActionClassContext context)
            => this.ReportError(
                "SD101",
                "Action identifiers must be valid uniform type identifiers (UTI)",
                $"Action '{{0}}' must have a valid UUID; identifiers can only contain lowercase alphanumeric characters (a-z, 0-9), hyphens (-), and periods (.).",
                context.Symbol.Locations,
                context.Name);

        /// <summary>
        /// Reports the <see cref="ActionAttribute.StateImage"/> is not defined on the <see cref="ActionAttribute"/>.
        /// </summary>
        /// <param name="context">The context containing information about the class declaration of the action.</param>
        public void ReportStateImageNotDefined(ActionClassContext context)
            => this.ReportError(
                "SD102",
                "State image must be defined",
                $"Action '{{0}}' must have a state image; set the '{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}', or add a '{nameof(StateAttribute)}'.",
                context.Symbol.Locations,
                context.Name);

        /// <summary>
        /// Reports the action contains both an <see cref="ActionAttribute.StateImage"/> and one or more <see cref="StateAttribute"/>.
        /// </summary>
        /// <param name="context">The context containing information about the class declaration of the action.</param>
        public void ReportStateImageDefinedMoreThanOnce(ActionClassContext context)
            => this.ReportError(
                "SD103",
                "State must not be defined more than once",
                $"Action '{{0}}' must not set the '{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}' when a '{nameof(StateAttribute)}' is present.",
                context.Symbol.Locations,
                context.Name);

        /// <summary>
        /// Reports the action has more than two <see cref="StateAttribute"/>.
        /// </summary>
        /// <param name="context">The context containing information about the class declaration of the action.</param>
        public void ReportActionHasTooManyStates(ActionClassContext context)
            => this.ReportError(
                "SD104",
                "Actions cannot have more than two states",
                $"Action '{{0}}' cannot have more than two states ('{nameof(StateAttribute)}').",
                context.Symbol.Locations,
                context.Name);

        /// <summary>
        /// Reports a <see cref="Diagnostic"/> with a severity of <see cref="DiagnosticSeverity.Error"/> error.
        /// </summary>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="locations">The optional <see cref="Diagnostic.Location"/>; when more than one, the first is taken.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        private void ReportError(string id, string title, string messageFormat, IEnumerable<Location>? locations = null, params string?[] messageArgs)
        {
            this.HasErrorDiagnostic = true;
            this.Context.ReportDiagnostic(
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
        }

        /// <summary>
        /// Reports a <see cref="Diagnostic"/> with a severity of <see cref="DiagnosticSeverity.Warning"/> error.
        /// </summary>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="locations">The optional <see cref="Diagnostic.Location"/>; when more than one, the first is taken.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        private void ReportWarning(string id, string title, string messageFormat, IEnumerable<Location>? locations = null, params string[] messageArgs)
            => this.Context.ReportDiagnostic(
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
    }
}
