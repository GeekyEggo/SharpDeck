namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        /// <summary>
        /// Reports a <see cref="DiagnosticSeverity.Error"/> due to a required value field within the manifest being null.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute that contains the data.</typeparam>
        /// <param name="node">The node of the attribute.</param>
        /// <param name="propertyName">Name of the property.</param>
        public void ReportValueCannotBeNull<TAttribute>(AttributeSyntax node, string propertyName)
            => this.ReportValueCannotBeNull<TAttribute>(node.GetLocation(), propertyName);

        #region Manifest

        /// <summary>
        /// Reports a <see cref="DiagnosticSeverity.Error"/> due to a required value field within the manifest being null.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute that contains the data.</typeparam>
        /// <param name="location">The location the error occured.</param>
        /// <param name="propertyName">Name of the property.</param>
        public void ReportValueCannotBeNull<TAttribute>(Location location, string propertyName)
            => this.ReportError(
                id: "SDM001",
                title: $"Required manifest information cannot be null",
                messageFormat: "Stream Deck {0} '{1}' cannot be null",
                locations: new[] { location },
                messageArgs: new[]
                {
                    typeof(TAttribute).Name.Substring(0, typeof(TAttribute).Name.Length - 9).ToLowerInvariant(),
                    propertyName,
                });

        /// <summary>
        /// Reports a <see cref="DiagnosticSeverity.Warning"/> due to a required value field within the manifest not being defined.
        /// </summary>
        /// <param name="node">The <see cref="AttributeSyntax"/> that contains the data.</param>
        /// /// <param name="propertyName">Name of the property.</param>
        public void ReportValueNotDefined<TAttribute>(AttributeSyntax node, string propertyName)
            => this.ReportWarning(
                id: "SDM051",
                title: $"Required manifest information is not defined",
                messageFormat: "Stream Deck {0} '{1}' is not defined; consider setting '{2}.{1}'",
                locations: new[] { node.GetLocation() },
                messageArgs: new[]
                {
                    typeof(TAttribute).Name.Substring(0, typeof(TAttribute).Name.Length - 9).ToLowerInvariant(),
                    propertyName,
                    typeof(TAttribute).Name
                });

        /// <summary>
        /// Reports a <see cref="DiagnosticSeverity.Warning"/> due to the <see cref="ActionAttribute.StateImage"/> not being required as <see cref="StateAttribute"/> is present.
        /// </summary>
        /// <param name="location">The location of the definition.</param>
        public void ReportActionStateImageNotRequired(Location location)
            => this.ReportWarning(
                id: "SDM052",
                title: $"'{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}' is not required",
                messageFormat: "Stream Deck action '{0}' can be removed as one or more '{1}' are present",
                locations: new[] { location },
                messageArgs: new[] { nameof(ActionAttribute.StateImage), nameof(StateAttribute) });

        /// <summary>
        /// Reports a <see cref="DiagnosticSeverity.Warning"/> due to the <see cref="ActionAttribute"/> having too many <see cref="StateAttribute"/>.
        /// </summary>
        /// <param name="stateContext">The context that contains information about the state.</param>
        public void ReportActionStateIgnored(AttributeContext stateContext)
            => this.ReportWarning(
                id: "SDM053",
                title: "Action has too many states",
                messageFormat: "Stream Deck actions cannot have more than 2 states",
                locations: new[] { stateContext.Node.GetLocation() });

        #endregion

        #region Manifest JSON Generation

        /// <summary>
        /// Reports the project directory could not be found when attempting to generate the manifest.json file.
        /// </summary>
        /// <param name="locations">The locations associated with the assembly that attempted to generate the manifest.json file..</param>
        public void ReportProjectDirectoryNotFoundForManifestJson(IEnumerable<Location> locations)
            => this.ReportError(
                id: "SDP001",
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
