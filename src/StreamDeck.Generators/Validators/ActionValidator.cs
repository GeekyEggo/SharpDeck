namespace StreamDeck.Generators.Validators
{
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Extensions;

    /// <summary>
    /// Provides validation for <see cref="ActionAttribute"/>, and reports all diagnostic findings to <see cref="GeneratorExecutionContext"/>.
    /// </summary>
    internal class ActionValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionValidator"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ActionValidator(GeneratorExecutionContext context)
            => this.Context = context;

        /// <summary>
        /// Gets the context.
        /// </summary>
        public GeneratorExecutionContext Context { get; }

        /// <summary>
        /// Validates the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="states">The states.</param>
        /// <param name="location">The location of the declaration.</param>
        /// <returns><c>true</c> when the action is valid; otherwise <c>false</c>.</returns>
        public bool Validate(ActionAttribute action, StateAttribute[] states, Location? location)
        {
            var isActionValid = true;

            // Validate UUID characters (https://developer.elgato.com/documentation/stream-deck/sdk/manifest/).
            if (Regex.IsMatch(action.UUID, @"[^a-z0-9\-\.]+"))
            {
                isActionValid = false;
                this.Context.ReportError(
                    "SD101",
                    "Action identifiers must be valid uniform type identifiers (UTI)",
                    $"Action '{{0}}' must have a valid UUID; identifiers can only contain lowercase alphanumeric characters (a-z, 0-9), hyphens (-), and periods (.).",
                    location,
                    action.Name);
            }

            // Validate the state image is defined.
            if (action.StateImage == null
                && states.Length == 0)
            {
                isActionValid = false;
                this.Context.ReportError(
                    "SD102",
                    "State image must be defined",
                    $"Action '{{0}}' must have a state image; set the '{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}', or add a '{nameof(StateAttribute)}'.",
                    location,
                    action.Name);
            }

            // Validate the state image is not defined more than once.
            if (action.StateImage != null
                && states.Length > 0)
            {
                isActionValid = false;
                this.Context.ReportError(
                    "SD103",
                    "State must not be defined more than once",
                    $"Action '{{0}}' must not set the '{nameof(ActionAttribute)}.{nameof(ActionAttribute.StateImage)}' when a '{nameof(StateAttribute)}' is present.",
                    location,
                    action.Name);
            }

            // Validate there are not more than 2 states on the action.
            if (states.Length > 2)
            {
                isActionValid = false;
                this.Context.ReportError(
                    "SD104",
                    "Actions cannot have more than two states",
                    $"Action '{{0}}' cannot have more than two states ('{nameof(StateAttribute)}').",
                    location,
                    action.Name);
            }

            return isActionValid;
        }
    }
}
