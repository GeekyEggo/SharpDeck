namespace SharpDeck.Manifest
{
    using SharpDeck.Enums;
    using System;

    /// <summary>
    /// Provides information about the possible state of a Stream Deck action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class StreamDeckActionStateAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionStateAttribute"/> class.
        /// </summary>
        /// <param name="image">The default image for the state.</param>
        public StreamDeckActionStateAttribute(string image)
        {
            this.Image = image;
        }

        /// <summary>
        /// Gets or sets the default image for the state.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this can be used if you want to provide a different image for the state when the action is displayed in a Multi Action.
        /// </summary>
        public bool? MultiActionImage { get; set; } = null;

        /// <summary>
        /// Gets or sets the name of the state displayed in the dropdown menu in the Multi action. For example Start or Stop for the states of the Game Capture Record action. If the name is not provided, the state will not appear in the Multi Action.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// Gets or sets default title.
        /// </summary>
        public string Title { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether to hide/show the title. True by default.
        /// </summary>
        public bool? ShowTitle { get; set; } = null;

        /// <summary>
        /// Gets or sets default title color.
        /// </summary>
        public string TitleColor { get; set; } = null;

        /// <summary>
        /// Gets or sets default title vertical alignment. Possible values are "top", "bottom" and "middle".
        /// </summary>
        public TitleAlignmentType? TitleAlignment { get; set; } = null;

        /// <summary>
        /// Gets or sets default font family for the title. Possible values are "Arial", "Arial Black", "Comic Sans MS", "Courier", "Courier New", "Georgia", "Impact", "Microsoft Sans Serif", "Symbol", "Tahoma", "Times New Roman", "Trebuchet MS", "Verdana", "Webdings", "Wingdings".
        /// </summary>
        public FontFamilyType? FontFamily { get; set; } = null;

        /// <summary>
        /// Gets or sets default font style for the title. Possible values are "Regular", "Bold", "Italic" and "Bold Italic". Note that some fonts might not support all values.
        /// </summary>
        public FontStyleType? FontStyle { get; set; } = null;

        /// <summary>
        /// Gets or sets default font size for the title.
        /// </summary>
        public int? FontSize { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether to have an underline under the title. False by default
        /// </summary>
        public bool? FontUnderline { get; set; } = null;

    }
}
