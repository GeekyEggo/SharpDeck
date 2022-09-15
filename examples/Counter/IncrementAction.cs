namespace Counter
{
    using System.Threading.Tasks;
    using StreamDeck.Events;

    /// <summary>
    /// Defines the action that increments the count.
    /// </summary>
    [Action(
        Name = "Increment",
        Icon = "Images/Action",
        SupportedInMultiActions = false,
        Tooltip = "Increment the count by one.")]
    [State(
        image: "Images/Key",
        TitleAlignment = TitleAlignment.Middle,
        FontFamily = FontFamily.Default,
        FontSize = "18")]
    public class IncrementAction : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementAction"/> class.
        /// </summary>
        /// <param name="context">The action's initialization context.</param>
        public IncrementAction(ActionInitializationContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override Task OnWillAppear(ActionEventArgs<ActionPayload> args)
            => base.OnWillAppear(args);
    }
}
