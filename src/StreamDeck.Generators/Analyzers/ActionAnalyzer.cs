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
        public ActionAnalyzer(ActionClassContext context, Manifest? manifest)
        {
            this.Context = context;
            this.Action = context.ActionAttribute.Data.CreateInstance<ActionAttribute>();

            this.SetDefaultValues(manifest);
            this.AddStates();
        }

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
        /// Assigns the default values to the <see cref="ActionAttribute"/>.
        /// </summary>
        private void SetDefaultValues(Manifest? manifest)
        {
            // Icon.
            if (this.Action.Icon == null)
            {
                this.Action.Icon = string.Empty;
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
                    // todo: Warn no states defined.
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
