namespace StreamDeck.Events
{
    using System.Text.Json.Nodes;

    /// <summary>
    /// Provides payload information about a title.
    /// </summary>
    public class TitlePayload : ActionPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPayload" /> class.
        /// </summary>
        /// <param name="title">The new title.</param>
        /// <param name="titleParameters">The title parameters describing the title.</param>
        /// <param name="coordinates">The coordinates of a triggered action.</param>
        /// <param name="isInMultiAction">A value indicating whether the action is inside a Multi Action.</param>
        /// <param name="settings">The JSON containing data that you can set and are stored persistently.</param>
        public TitlePayload(string title, TitleParameters titleParameters, Coordinates coordinates, bool isInMultiAction, JsonObject settings)
            : base(coordinates, isInMultiAction, settings)
        {
            this.Title = title;
            this.TitleParameters = titleParameters;
        }

        /// <summary>
        /// Gets the new title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the title parameters describing the title.
        /// </summary>
        public TitleParameters TitleParameters { get; }
    }
}
