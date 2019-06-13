namespace SharpDeck.Manifest
{
    using System;

    /// <summary>
    /// Provides information about a Stream Deck action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class StreamDeckActionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="uuid">The unique identifier of the action.</param>
        /// <param name="icon">The relative path to a PNG image without the .png extension; <see cref="Icon"/> is required unless <see cref="VisibleInActionsList"/> is set to <c>false</c>..</param>
        public StreamDeckActionAttribute(string name, string uuid, string icon = "")
        {
            this.Icon = icon;
            this.Name = name;
            this.UUID = uuid;
        }

        /// <summary>
        /// Gets or sets the relative path to a PNG image without the .png extension. This image is displayed in the actions list. The PNG image should be a 20pt x 20pt image. Ideally you should provide a @1x and @2x version of the image. The Stream Deck application take care of loaded the appropriate version of the image. This icon is not required for actions not visible in the actions list (VisibleInActionsList set to false).
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets this can override PropertyInspectorPath member from the plugin if you wish to have different PropertyInspectorPath based on the action. The relative path to the Property Inspector html file if your plugin want to display some custom settings in the Property Inspector.
        /// </summary>
        public string PropertyInspectorPath { get; set; }

        /// <summary>
        /// Gets or sets boolean to prevent the action from being used in a Multi Action. True by default.
        /// </summary>
        public bool? SupportedInMultiActions { get; set; } = null;

        /// <summary>
        /// Gets or sets the string displayed as tooltip when the user leaves the mouse over your action in the actions list.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the action. It must be a uniform type identifier (UTI) that contains only alphanumeric characters (A-Z, a-z, 0-9), hyphen (-), and period (.). The string must be in reverse-DNS format. For example, if your domain is elgato.com and you create a plugin named Hello with the action My Action, you could assign the string com.elgato.hello.myaction as your action's Unique Identifier.
        /// </summary>
        public string UUID { get; set; }

        /// <summary>
        /// Gets or sets boolean to hide the action in the actions list. This can be used for plugin that only works with a specific profile. True by default.
        /// </summary>
        public bool? VisibleInActionsList { get; set; } = null;
    }
}
