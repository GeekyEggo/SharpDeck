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
        public ActionAnalyzer(ActionClassContext context, Manifest? manifest, DiagnosticReporter diagnosticReporter)
        {
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
                static string GetSegmnet(string? value)
                    => value is null ? string.Empty : Regex.Replace(value.ToLowerInvariant(), "[^a-z0-9\\-]+", string.Empty);

                this.Action.UUID = $"com.{GetSegmnet(manifest?.Author)}.{GetSegmnet(manifest?.Name)}.{GetSegmnet(this.Action.Name)}";
            }

            this.HasValidUUID = Regex.IsMatch(this.Action.UUID, @"^[a-z0-9\-]+\.[a-z0-9\-]+\.[a-z0-9\-]+\.[a-z0-9\-]+$");
        }

        /// <summary>
        /// Assigns the <see cref="StateAttribute"/> to <see cref="ActionAttribute.States"/>.
        /// </summary>
        private void AddStates()
        {
            if (this.Context.StateAttributes.Count == 0)
            {
                if (this.Action.States.Count == 0)
                {
                    this.Action.StateImage = string.Empty;
                    this.DiagnosticReporter.ReportActionStateImageMissing(this.Context.ActionAttribute);
                }

                return;
            }

            this.Action.States.Clear();
            foreach (var state in this.Context.StateAttributes.Select(s => s.Data.CreateInstance<StateAttribute>()))
            {
                if (string.IsNullOrWhiteSpace(state.Image))
                {
                    state.Image = string.Empty;
                }

                if (this.Action.States.Count < 2)
                {
                    this.Action.States.Add(state);
                }
                else
                {
                    // todo: Warn state is ignored.
                }
            }
        }
    }
}
