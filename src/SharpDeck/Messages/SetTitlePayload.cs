namespace SharpDeck.Messages
{
    using Enums;

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
        public SetTitlePayload(string title = "", TargetType target = TargetType.Both)
            : base(target)
        {
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets the title to display; when no title is passed, the title is reset to the default title from the manifest.
        /// </summary>
        public string Title { get; set; }
    }
}
