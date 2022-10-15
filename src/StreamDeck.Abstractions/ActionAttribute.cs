namespace StreamDeck
{
    using System;
    using System.Runtime.Serialization;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides information about an action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class ActionAttribute : SerializableSafeAttribute
    {
        /// <summary>
        /// Private member field for <see cref="StateImage"/>.
        /// </summary>
        private string? _stateIcon;

        /// <summary>
        /// Gets or sets a value indicating to disable image caching. <c>false</c> by default.
        /// </summary>
        [IgnoreDataMemberWhen(false)]
        public bool DisableCaching { get; set; } = false;

        /// <summary>
        /// Gets or sets the relative path to a PNG image without the .png extension. This image is displayed in the actions list. The PNG image should be a 20pt x 20pt image. You should provide @1x and @2x versions of the image. The Stream Deck application takes care of loading the appropriate version of the image. This icon is not required for actions not visible in the actions list (VisibleInActionsList set to false).
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the action. This string is visible to the user in the actions list.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets property inspector path; this can override PropertyInspectorPath member from the plugin if you wish to have a different PropertyInspectorPath based on the action. The relative path to the Property Inspector HTML file if your plugin wants to display some custom settings in the Property Inspector.
        /// </summary>
        public string? PropertyInspectorPath { get; set; }

        /// <summary>
        /// Gets or sets the sort index, used to determine the order actions are shown. By default, actions with a <see cref="SortIndex"/> are shown first, followed by actions in alphabetical order by their <see cref="Name"/>.
        /// </summary>
        [IgnoreDataMember]
        public int SortIndex { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the default image for the only state; to define more information about a state, or define multiple states, please use the <see cref="StateAttribute"/>.
        /// </summary>
        [IgnoreDataMember]
        public string? StateImage
        {
            get => this._stateIcon;
            set
            {
                this._stateIcon = value;
                this.States = new List<StateAttribute>() { new StateAttribute(value ?? string.Empty) };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to prevent the action from being used in a Multi Action. <c>true</c> by default.
        /// </summary>
        [IgnoreDataMemberWhen(true)]
        public bool SupportedInMultiActions { get; set; } = true;

        /// <summary>
        /// Gets or sets the string to display as a tooltip when the user leaves the mouse over your action in the actions list.
        /// </summary>
        public string? Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the action. It must be a uniform type identifier (UTI) that contains only lowercase alphanumeric characters (a-z, 0-9), hyphen (-), and period (.). The string must be in reverse-DNS format. For example, if your domain is elgato.com and you create a plugin named Hello with the action My Action, you could assign the string com.elgato.hello.myaction as your action's Unique Identifier.
        /// </summary>
        public string UUID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to hide the action in the actions list. This can be used for a plugin that only works with a specific profile. <c>true</c> by default.
        /// </summary>
        [IgnoreDataMemberWhen(true)]
        public bool VisibleInActionsList { get; set; } = true;

        /// <summary>
        /// Gets or sets the information about the states of the action.
        /// </summary>
        internal List<StateAttribute> States { get; set; } = new List<StateAttribute>();
    }
}
