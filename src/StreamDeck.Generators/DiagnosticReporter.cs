namespace StreamDeck.Generators
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck.Generators.Extensions;

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
        /// The link to the Stream Deck developer documentation; Manifest Actions.
        /// </summary>
        private const string MANIFEST_ACTIONS_HELP_LINK = "https://developer.elgato.com/documentation/stream-deck/sdk/manifest/#actions";

        /// <summary>
        /// The link to the Stream Deck developer documentation; Manifest States.
        /// </summary>
        private const string MANIFEST_STATES_HELP_LINK = "https://developer.elgato.com/documentation/stream-deck/sdk/manifest/#states";

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

        #region ManifestJsonGenerator

        /// <summary>
        /// Reports the project directory could not be found when attempting to generate the manifest.json file.
        /// </summary>
        /// <param name="locations">The locations associated with the assembly that attempted to generate the manifest.json file..</param>
        public void ReportProjectDirectoryNotFoundForManifestJson(IEnumerable<Location> locations)
            => this.Report(
                DiagnosticSeverity.Error,
                "SDJ001",
                "Generating a manifest.json requires a project directory",
                "Failed to generate manifest JSON file as the project's directory is unknown. Consider creating a manifest.json file manually.",
                MANIFEST_HELP_LINK,
                locations);

        #endregion

        /*
        /// <summary>
        /// Reports the <see cref="ManifestAttribute"/> is contains information that was ignored when serializing as it was not valid.
        /// </summary>
        /// <param name="memberName">The name of the member that is required.</param>
        /// <param name="manifestNode">The <see cref="AttributeSyntax"/> that represents the <see cref="ManifestAttribute"/>.</param>
        internal void ReportManifestInformationIgnored(string memberName, AttributeSyntax manifestNode)
            => this.Report(
                DiagnosticSeverity.Warning,
                "SD002",
                "Manifest information ignored",
                "The '{0}' value was ignored when creating the manifest.json file; value should not be null or empty.",
                MANIFEST_HELP_LINK,
                new[] { manifestNode.GetNamedArgumentLocationOrDefault(memberName) },
                memberName);

        /// <summary>
        /// Reports the <see cref="ManifestAttribute"/> is missing required information that cannot be fulfilled by another attribute.
        /// </summary>
        /// <param name="memberName">The name of the member that is required.</param>
        /// <param name="manifestNode">The <see cref="AttributeSyntax"/> that represents the <see cref="ManifestAttribute"/>.</param>
        internal void ReportManifestRequires(string memberName, AttributeSyntax manifestNode)
            => this.Report(
                DiagnosticSeverity.Warning,
                "SD003",
                "Manifest is missing required information",
                "The manifest.json file requires the '{0}' to be specified.",
                MANIFEST_HELP_LINK,
                new[] { manifestNode.GetNamedArgumentLocationOrDefault(memberName) },
                memberName);

        /// <summary>
        /// Reports the <see cref="Models.Manifest.Actions"/> contains no actions.
        /// </summary>
        /// <param name="manifestNode">The <see cref="AttributeSyntax"/> that represents the <see cref="ManifestAttribute"/>.</param>
        internal void ReportManifestRequiresActions(AttributeSyntax manifestNode)
            => this.Report(
                DiagnosticSeverity.Error,
                "SD004",
                "Actions are required",
                "No actions were found when generating the manifest.json file; consider adding '{0}' to a class definition.",
                MANIFEST_HELP_LINK,
                new[] { manifestNode.GetLocation() },
                nameof(ActionAttribute),
                nameof(StateAttribute));

        /// <summary>
        /// Reports the <see cref="ManifestAttribute.Author"/> is <c>null</c> or empty.
        /// </summary>
        /// <param name="manifestNode">The <see cref="AttributeSyntax"/> that represents the <see cref="ManifestAttribute"/>.</param>
        internal void ReportManifestRequiresAuthor(AttributeSyntax manifestNode)
            => this.Report(
                DiagnosticSeverity.Warning,
                "SD005",
                "Manifest requires an Author or AssemblyCompanyAttribute",
                "The manifest.json file requires an '{0}'; consider specifying the '{0}', or adding '{1}'.",
                MANIFEST_HELP_LINK,
                new[] { manifestNode.GetNamedArgumentLocationOrDefault(nameof(ManifestAttribute.Author)) },
                nameof(ManifestAttribute.Author),
                nameof(System.Reflection.AssemblyCompanyAttribute));

        /// <summary>
        /// Reports the <see cref="ManifestAttribute.Description"/> is <c>null</c> or empty.
        /// </summary>
        /// <param name="manifestNode">The <see cref="AttributeSyntax"/> that represents the <see cref="ManifestAttribute"/>.</param>
        internal void ReportManifestRequiresDescription(AttributeSyntax manifestNode)
            => this.Report(
                DiagnosticSeverity.Warning,
                "SD006",
                "Manifest requires a Description or AssemblyDescriptionAttribute",
                "The manifest.json file requires a '{0}'; consider specifying the '{0}', or adding '{1}'.",
                MANIFEST_HELP_LINK,
                new[] { manifestNode.GetNamedArgumentLocationOrDefault(nameof(ManifestAttribute.Description)) },
                nameof(ManifestAttribute.Description),
                nameof(System.Reflection.AssemblyDescriptionAttribute));

        /// <summary>
        /// Reports the <see cref="ManifestAttribute.Name"/> is <c>null</c> or empty.
        /// </summary>
        /// <param name="manifestNode">The <see cref="AttributeSyntax"/> that represents the <see cref="ManifestAttribute"/>.</param>
        internal void ReportManifestRequiresName(AttributeSyntax manifestNode)
            => this.Report(
                DiagnosticSeverity.Warning,
                "SD007",
                "Manifest requires a Name or AssemblyProductAttribute",
                "The manifest.json file requires a '{0}'; consider specifying the '{0}', or adding '{1}'.",
                MANIFEST_HELP_LINK,
                new[] { manifestNode.GetNamedArgumentLocationOrDefault(nameof(ManifestAttribute.Name)) },
                nameof(ManifestAttribute.Name),
                nameof(System.Reflection.AssemblyProductAttribute));

        /// <summary>
        /// Reports the <see cref="ManifestAttribute.Version"/> is <c>null</c> or empty.
        /// </summary>
        /// <param name="manifestNode">The <see cref="AttributeSyntax"/> that represents the <see cref="ManifestAttribute"/>.</param>
        internal void ReportManifestRequiresVersion(AttributeSyntax manifestNode)
            => this.Report(
                DiagnosticSeverity.Warning,
                "SD008",
                "Manifest requires a Version or AssemblyVersionAttribute",
                "The manifest.json file requires a '{0}'; consider specifying the '{0}', or adding '{1}'.",
                MANIFEST_HELP_LINK,
                new[] { manifestNode.GetNamedArgumentLocationOrDefault(nameof(ManifestAttribute.Version)) },
                nameof(ManifestAttribute.Version),
                nameof(System.Reflection.AssemblyVersionAttribute));

        #endregion

        #region Actions

        /// <summary>
        /// Reports the <see cref="ActionAttribute.UUID"/> is invalid.
        /// </summary>
        /// <param name="context">The context containing information about the class declaration of the action.</param>
        internal void ReportInvalidActionUUID(ActionClassContext context)
            => this.Report(
                DiagnosticSeverity.Error,
                "SD101",
                "Action identifiers must be valid uniform type identifiers (UTI)",
                $"Action '{{0}}' must have a valid UUID; identifiers can only contain lowercase alphanumeric characters (a-z, 0-9), hyphens (-), and periods (.).",
                MANIFEST_ACTIONS_HELP_LINK,
                context.Symbol.Locations,
                context.Name);

        /// <summary>
        /// Reports the <see cref="ActionAttribute.StateImage"/> is not defined on the <see cref="ActionAttribute"/>.
        /// </summary>
        /// <param name="context">The context containing information about the class declaration of the action.</param>
        internal void ReportStateImageNotDefined(ActionClassContext context)
            => this.Report(
                DiagnosticSeverity.Warning,
                "SD102",
                "State image must be defined",
                $"Action '{{0}}' must have a state image; set the '{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}', or add a '{nameof(StateAttribute)}'.",
                MANIFEST_ACTIONS_HELP_LINK,
                context.Symbol.Locations,
                context.Name);

        /// <summary>
        /// Reports the action contains both an <see cref="ActionAttribute.StateImage"/> and one or more <see cref="StateAttribute"/>.
        /// </summary>
        /// <param name="context">The context containing information about the class declaration of the action.</param>
        internal void ReportStateImageDefinedMoreThanOnce(ActionClassContext context)
            => this.Report(
                DiagnosticSeverity.Error,
                "SD103",
                "State must not be defined more than once",
                $"Action '{{0}}' must not set the '{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}' when a '{nameof(StateAttribute)}' is present.",
                locations: context.Symbol.Locations,
                messageArgs: new[] { context.Name });

        /// <summary>
        /// Reports the action has more than two <see cref="StateAttribute"/>.
        /// </summary>
        /// <param name="context">The context containing information about the class declaration of the action.</param>
        internal void ReportActionHasTooManyStates(ActionClassContext context)
            => this.Report(
                DiagnosticSeverity.Error,
                "SD104",
                "Actions cannot have more than two states",
                $"Action '{{0}}' cannot have more than two states ('{nameof(StateAttribute)}').",
                MANIFEST_STATES_HELP_LINK,
                context.StateAttributes.Skip(2).Select(s => s.Node.GetLocation()),
                context.Name);

        #endregion
        */
        /// <summary>
        /// Reports a <see cref="Diagnostic"/>.
        /// </summary>
        /// <param name="severity">The <see cref="DiagnosticSeverity"/>.</param>
        /// <param name="id">The <see cref="Diagnostic.Id"/>.</param>
        /// <param name="title">The <see cref="DiagnosticDescriptor.Title"/>.</param>
        /// <param name="messageFormat">The <see cref="DiagnosticDescriptor.MessageFormat"/>.</param>
        /// <param name="helpLinkUri">The optional help link.</param>
        /// <param name="locations">The optional <see cref="Diagnostic.Location"/>; when more than one, the first is taken.</param>
        /// <param name="messageArgs">The optional message arguments supplied to the message format when generating the description.</param>
        private void Report(DiagnosticSeverity severity, string id, string title, string messageFormat, string? helpLinkUri = null, IEnumerable<Location>? locations = null, params string?[] messageArgs)
        {
            if (severity == DiagnosticSeverity.Error)
            {
                this.HasErrorDiagnostic = true;
            }

            foreach (var location in locations ?? new Location[] { null! })
            {
                this.Context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: id,
                            title: title,
                            messageFormat: messageFormat,
                            category: DIAGNOSTIC_CATEGORY,
                            defaultSeverity: severity,
                            isEnabledByDefault: true,
                            helpLinkUri: helpLinkUri),
                        location,
                        messageArgs));
            }
        }
    }
}
