namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Analyzers;

    /// <summary>
    /// Provides methods for reporting and monitoring diagnostics reported to a <see cref="GeneratorExecutionContext"/>.
    /// </summary>
    internal class DiagnosticReporter
    {
        /// <summary>
        /// The link to the Stream Deck developer documentation; Manifest.
        /// </summary>
        private const string MANIFEST_HELP_LINK = "https://developer.elgato.com/documentation/stream-deck/sdk/manifest";

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

        #region Manifest Analyzer

        /// <summary>
        /// Warns when the <see cref="ManifestAttribute.Author"/> has not been specified, and has defaulted to 'User'.
        /// </summary>
        /// <param name="attribute">The manifest attribute context.</param>
        public void ReportManifestAuthorMissing(AttributeContext attribute)
            => this.ReportWarning(
                id: "SD1001",
                title: $"Manifest '{nameof(ManifestAttribute.Author)}' is missing",
                messageFormat: "Manifest '{0}' has defaulted to 'User'; consider adding a <Company> element to the project file",
                locations: new[] { attribute.Node.GetLocation() },
                messageArgs: nameof(ManifestAttribute.Author));

        /// <summary>
        /// Warns when the <see cref="ManifestAttribute.Description"/> has not been specified.
        /// </summary>
        /// <param name="attribute">The manifest attribute context.</param>
        public void ReportManifestDescriptionMissing(AttributeContext attribute)
            => this.ReportWarning(
                id: "SD1002",
                title: $"Manifest '{nameof(ManifestAttribute.Description)}' is missing",
                messageFormat: "Manifest '{0}' is not defined; consider adding a <Description> element to the project file",
                locations: new[] { attribute.Node.GetLocation() },
                messageArgs: nameof(ManifestAttribute.Description));

        /// <summary>
        /// Warns when the <see cref="ManifestAttribute.Icon"/> has not been specified.
        /// </summary>
        /// <param name="attribute">The manifest attribute context.</param>
        public void ReportManifestIconMissing(AttributeContext attribute)
             => this.ReportWarning(
                id: "SD1003",
                title: $"Manifest '{nameof(ManifestAttribute.Icon)}' is missing",
                messageFormat: "Manifest '{0}' is not defined; consider setting the '{1}.{0}'",
                locations: new[] { attribute.Node.GetLocation() },
                messageArgs: new[] { nameof(ManifestAttribute.Icon), nameof(ManifestAttribute) });

        #endregion

        #region Action Analyzer

        /// <summary>
        /// Warns when the <see cref="ActionAttribute.Icon"/> has not been specified.
        /// </summary>
        /// <param name="attribute">The action attribute context.</param>
        public void ReportActionIconMissing(AttributeContext attribute)
            => this.ReportWarning(
                id: "SD2001",
                title: $"Action '{nameof(ActionAttribute.Icon)}' is missing",
                messageFormat: "Action '{0}' is not defined; consider setting the '{1}.{0}'",
                locations: new[] { attribute.Node.GetLocation() },
                messageArgs: new[] { nameof(ActionAttribute.Icon), nameof(ActionAttribute) });

        /// <summary>
        /// Warns when the <see cref="ActionAttribute.StateImage"/> has not been specified.
        /// </summary>
        /// <param name="attribute">The action attribute context.</param>
        public void ReportActionStateImageMissing(AttributeContext attribute)
            => this.ReportWarning(
                id: "SD2002",
                title: $"Action '{nameof(ActionAttribute.StateImage)}' is missing",
                messageFormat: "Action '{0}' is not defined; consider setting the '{1}.{0}'",
                locations: new[] { attribute.Node.GetLocation() },
                messageArgs: new[] { nameof(ActionAttribute.StateImage), nameof(ActionAttribute) });

        #endregion

        #region Manifest JSON Generation

        /// <summary>
        /// Reports the project directory could not be found when attempting to generate the manifest.json file.
        /// </summary>
        /// <param name="locations">The locations associated with the assembly that attempted to generate the manifest.json file..</param>
        public void ReportProjectDirectoryNotFoundForManifestJson(IEnumerable<Location> locations)
            => this.ReportError(
                id: "SD1010",
                title: "Generating a manifest.json requires a project directory",
                messageFormat: "Failed to generate manifest JSON file as the project's directory is unknown. Consider creating a manifest.json file manually.",
                MANIFEST_HELP_LINK,
                locations);

        #endregion

        /// <summary>
        /// Reports a <see cref="Diagnostic"/> of <see cref="DiagnosticSeverity.Error"/>.
        /// </summary>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="helpLinkUri">The optional help link.</param>
        /// <param name="locations">The optional <see cref="Diagnostic.Location"/>; when more than one, the first is taken.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        private void ReportError(string id, string title, string messageFormat, string? helpLinkUri = null, IEnumerable<Location>? locations = null, params string?[] messageArgs)
        {
            this.HasErrorDiagnostic = true;
            this.Report(id, title, messageFormat, DiagnosticSeverity.Error, helpLinkUri, locations, messageArgs);
        }

        /// <summary>
        /// Reports a <see cref="Diagnostic"/> of <see cref="DiagnosticSeverity.Warning"/>.
        /// </summary>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="defaultSeverity">The <see cref="DiagnosticSeverity"/>.</param>
        /// <param name="helpLinkUri">The optional help link.</param>
        /// <param name="locations">The optional <see cref="Diagnostic.Location"/>; when more than one, the first is taken.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        private void ReportWarning(string id, string title, string messageFormat, string? helpLinkUri = null, IEnumerable<Location>? locations = null, params string?[] messageArgs)
            => this.Report(id, title, messageFormat, DiagnosticSeverity.Warning, helpLinkUri, locations, messageArgs);

        /// <summary>
        /// Reports a <see cref="Diagnostic"/>.
        /// </summary>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="defaultSeverity">The <see cref="DiagnosticSeverity"/>.</param>
        /// <param name="helpLinkUri">The optional help link.</param>
        /// <param name="locations">The optional <see cref="Diagnostic.Location"/>; when more than one, the first is taken.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        private void Report(string id, string title, string messageFormat, DiagnosticSeverity defaultSeverity, string? helpLinkUri = null, IEnumerable<Location>? locations = null, params string?[] messageArgs)
            => this.Context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: id,
                        title: title,
                        messageFormat: messageFormat,
                        category: DIAGNOSTIC_CATEGORY,
                        defaultSeverity: defaultSeverity,
                        isEnabledByDefault: true,
                        helpLinkUri: helpLinkUri),
                    locations.FirstOrDefault(),
                    locations?.Skip(1) ?? Enumerable.Empty<Location>(),
                    messageArgs));
    }
}
