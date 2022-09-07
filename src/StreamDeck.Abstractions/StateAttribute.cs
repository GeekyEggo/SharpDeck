namespace StreamDeck
{
    using System;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides information about the state of an action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class StateAttribute : SerializableSafeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateAttribute"/> class.
        /// </summary>
        /// <param name="image">The default image for the state.</param>
        public StateAttribute(string image)
            : base()
            => this.Image = image;

        /// <summary>
        /// Gets the default image for the state.
        /// </summary>
        public string Image { get; }

        /// <summary>
        /// Gets or sets the default font family for the title
        /// </summary>
        public string? FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the default font size for the title.
        /// </summary>
        /// <example>13</example>
        public string? FontSize { get; set; }

        /// <summary>
        /// Gets or sets the default font style for the title. Note that some fonts might not support all values.
        /// </summary>
        public string? FontStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to have an underline under the title; <c>false</c> by default
        /// </summary>
        [IgnoreDataMemberWhen(false)]
        public bool FontUnderline { get; set; } = false;

        /// <summary>
        /// Gets or sets the multi-action image; this can be used if you want to provide a different image for the state when the action is displayed in a Multi-Action.
        /// </summary>
        public string? MultiActionImage { get; set; }

        /// <summary>
        /// Gets or sets the name that is displayed in the dropdown menu in the Multi-action. For example, the Game Capture Record action has Start and Stop. If the name is not provided, the state will not appear in the Multi-Action.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide/show the title; <c>true</c> by default.
        /// </summary>
        [IgnoreDataMemberWhen(true)]
        public bool ShowTitle { get; set; } = true;

        /// <summary>
        /// Gets or sets the default title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the default title color.
        /// </summary>
        public string? TitleColor { get; set; }

        /// <summary>
        /// Gets or sets the default title vertical alignment.
        /// </summary>
        public string? TitleAlignment { get; set; }
    }
}
