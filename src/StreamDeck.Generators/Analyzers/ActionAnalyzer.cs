namespace StreamDeck.Generators.Analyzers
{
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.Models;

    /// <summary>
    /// Provides an analyzer that constructs and validates a <see cref="ActionAttribute"/>.
    /// </summary>
    internal class ActionAnalyzer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionAnalyzer"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ActionClassContext"/>.</param>
        /// <param name="manifest">The <see cref="Manifest"/>.</param>
        /// <param name="diagnosticReporter">The diagnostic reporter.</param>
        public ActionAnalyzer(GeneratorExecutionContext generatorContext, ActionClassContext context, Manifest? manifest, DiagnosticReporter diagnosticReporter)
        {
            this.GeneratorContext = generatorContext;
            this.Context = context;
            this.DiagnosticReporter = new DiagnosticReporter(diagnosticReporter);
            this.Action = context.ActionAttribute.Data.CreateInstance<ActionAttribute>();

            this.SetDefaultValues(manifest);
            this.AddStates();
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public ActionClassContext Context { get; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        public ActionAttribute Action { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has a valid UUID.
        /// </summary>
        public bool HasValidUUID { get; private set; }

        /// <summary>
        /// Gets the diagnostic reporter.
        /// </summary>
        private DiagnosticReporter DiagnosticReporter { get; }

        /// <summary>
        /// Gets the generator context.
        /// </summary>
        private GeneratorExecutionContext GeneratorContext { get; }

        /// <summary>
        /// Gets the first safe UUID segment from the specified <paramref name="values"/>, that isn't <see cref="string.Empty"/>.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>The first safe UUID segment; otherwise <see cref="string.Empty"/>.</returns>
        static string GetUUIDSegment(params string?[] values)
        {
            foreach (var value in values)
            {
                var safeValue = Regex.Replace(value?.ToLowerInvariant() ?? string.Empty, "[^a-z0-9\\-]+", string.Empty);
                if (!string.IsNullOrWhiteSpace(safeValue))
                {
                    return safeValue;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Assigns the default values to the <see cref="ActionAttribute"/>.
        /// </summary>
        private void SetDefaultValues(Manifest? manifest)
        {
            // Icon.
            if (string.IsNullOrWhiteSpace(this.Action.Icon))
            {
                this.Action.Icon = string.Empty;
                this.DiagnosticReporter.ReportActionIconMissing(this.Context.ActionAttribute);
            }

            // Name.
            if (string.IsNullOrWhiteSpace(this.Action.Name))
            {
                this.Action.Name = this.Context.Symbol.Name;
            }

            // UUID.
            if (string.IsNullOrWhiteSpace(this.Action.UUID))
            {
                this.Action.UUID = $"com.{GetUUIDSegment(manifest?.Author, this.GeneratorContext.Compilation.Assembly.Identity.Name, "user")}.{GetUUIDSegment(manifest?.Name, this.GeneratorContext.Compilation.Assembly.Identity.Name, "plugin")}.{GetUUIDSegment(this.Action.Name)}";
            }

            this.HasValidUUID = Regex.IsMatch(this.Action.UUID, @"^[a-z0-9\-]+\.[a-z0-9\-]+\.[a-z0-9\-]+\.[a-z0-9\-]+$");
        }

        /// <summary>
        /// Assigns the <see cref="StateAttribute"/> to <see cref="ActionAttribute.States"/>.
        /// </summary>
        private void AddStates()
        {
            // When no state attributes are defined, build one from the StateImage on the action attribute.
            if (this.Context.StateAttributes.Count == 0)
            {
                if (this.Action.States.Count == 0)
                {
                    this.Action.StateImage = string.Empty;
                    this.DiagnosticReporter.ReportActionStateImageMissing(this.Context.ActionAttribute);
                }

                return;
            }

            // When there are state attributes defined; warn if StateImage is also defined.
            if (this.Context.ActionAttribute.Node.TryGetNamedArgument(nameof(ActionAttribute.StateImage), out var arg))
            {
                this.DiagnosticReporter.ReportActionStateImageNotRequired(arg!.GetLocation());
            }

            // Reset the states on the action, and build them from the state attributes.
            this.Action.States.Clear();
            foreach (var stateContext in this.Context.StateAttributes)
            {
                if (this.Action.States.Count >= 2)
                {
                    this.DiagnosticReporter.ReportActionStateIgnored(stateContext);
                }
                else
                {
                    var state = stateContext.Data.CreateInstance<StateAttribute>();
                    state.Image ??= string.Empty;

                    this.Action.States.Add(state);
                }
            }
        }
    }
}
