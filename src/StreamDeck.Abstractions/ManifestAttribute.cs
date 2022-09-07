namespace StreamDeck
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides information about the manifest of the plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class ManifestAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestAttribute"/> class.
        /// </summary>
        public ManifestAttribute()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the list of application identifiers to monitor (applications launched or terminated) on Mac. See the applicationDidLaunch and applicationDidTerminate events.
        /// </summary>
        [IgnoreDataMember]
        public string[] ApplicationsToMonitorMac { get; set; } = new string[0];

        /// <summary>
        /// Gets or sets the list of application identifiers to monitor (applications launched or terminated) on Windows. See the applicationDidLaunch and applicationDidTerminate events.
        /// </summary>
        [IgnoreDataMember]
        public string[] ApplicationsToMonitorWin { get; set; } = new string[0];

        /// <summary>
        /// Gets or sets the author of the plugin. This string is displayed to the user in the Stream Deck store.
        /// </summary>
        public string Author { get; set; } = "";

        /// <summary>
        /// Gets or sets the name of the custom category in which the actions should be listed. This string is visible to the user in the actions list. If you don't provide a category, the actions will appear inside a "Custom" category.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the relative path to a PNG image without the .png extension. This image is used in the actions list. The PNG image should be a 28pt x 28pt image. You should provide @1x and @2x versions of the image. The Stream Deck application takes care of loading the appropriate version of the image.
        /// </summary>
        public string? CategoryIcon { get; set; }

        /// <summary>
        /// Gets or sets the relative path to the HTML/binary file containing the plugin code.
        /// </summary>
        public string CodePath { get; set; } = "";

        /// <summary>
        /// Gets or sets override <see cref="CodePath"/> for macOS.
        /// </summary>
        public string? CodePathMac { get; set; }

        /// <summary>
        /// Gets or sets override <see cref="CodePath"/> for Windows.
        /// </summary>
        public string? CodePathWin { get; set; }

        /// <summary>
        /// Gets or sets the default window size when a Javascript plugin or Property Inspector opens a window using window.open(). The default value is [500, 650].
        /// </summary>
        public int[]? DefaultWindowSize { get; set; }

        /// <summary>
        /// Gets or sets a general description of what the plugin does. This string is displayed to the user in the Stream Deck store.
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Gets or sets the relative path to a PNG image without the .png extension. This image is displayed in the Plugin Store window. The PNG image should be a 72pt x 72pt image. You should provide @1x and @2x versions of the image. The Stream Deck application takes care of loading the appropriate version of the image.
        /// </summary>
        public string Icon { get; set; } = "";

        /// <summary>
        /// Gets or sets the name of the plugin. This string is displayed to the user in the Stream Deck store.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets minimum version of Mac supported by the plugin
        /// </summary>
        [IgnoreDataMember]
        public string OSMacMinimumVersion { get; set; } = "";

        /// <summary>
        /// Gets or sets minimum version of Windows supported by the plugin
        /// </summary>
        [IgnoreDataMember]
        public string OSWindowsMinimumVersion { get; set; } = "10";

        /// <summary>
        /// Gets or sets the relative path to the Property Inspector HTML file if your plugin wants to display some custom settings in the Property Inspector. If missing, the plugin will have an empty Property Inspector.
        /// </summary>
        public string? PropertyInspectorPath { get; set; }

        /// <summary>
        /// Gets or sets the SDK version.
        /// </summary>
        public int SDKVersion { get; set; } = 2;

        /// <summary>
        /// Gets or sets the value that indicates which version of the Stream Deck application is required to install the plugin.
        /// </summary>
        [IgnoreDataMember]
        public string SoftwareMinimumVersion { get; set; } = "5.0";

        /// <summary>
        /// Gets or sets a site to provide more information about the plugin.
        /// </summary>
        [DataMember(Name = "URL")]
        public string? Url { get; set; }

        /// <summary>
        /// Gets or sets the plugin's semantic version (1.0.0).
        /// </summary>
        public string Version { get; set; } = "";

        /// <inheritdoc/>
        [IgnoreDataMember]
        public override object TypeId => base.TypeId;
    }
}
