namespace StreamDeck.Events.Sent
{
    /// <summary>
    /// Provides payload information used to set a title.
    /// </summary>
    public class SetTitlePayload : TargetPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetTitlePayload"/> class.
        /// </summary>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        public SetTitlePayload(string title = "", TargetType target = TargetType.Both, int? state = null)
            : base(target, state) => this.Title = title;

        /// <summary>
        /// Gets the title to display; when no title is passed, the title is reset to the default title from the manifest.
        /// </summary>
        public string Title { get; }
    }
}
