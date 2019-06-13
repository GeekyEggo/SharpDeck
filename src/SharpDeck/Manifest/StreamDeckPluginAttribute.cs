namespace SharpDeck.Manifest
{
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Provides base information for an Elgato Stream Deck Plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class StreamDeckPluginAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the author of the plugin. This string is displayed to the user.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The name of the custom category in which the actions should be listed. This string is displayed to the user.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The relative path to a PNG image without the .png extension. This image is used in the actions list. The PNG image should be a 28pt x 28pt image. Ideally you should provide a @1x and @2x version of the image. The Stream Deck application takes care of loading the appropriate version of the image.
        /// </summary>
        public string CategoryIcon { get; set; }

        /// <summary>
        /// Gets or sets the relative path to the HTML/binary file containing the code of the plugin.
        /// </summary>
        public string CodePath { get; set; }

        /// <summary>
        /// Gets or sets the override CodePath for macOS.
        /// </summary>
        public string CodePathMac { get; set; }

        /// <summary>
        /// Gets or sets the override CodePath for Windows.
        /// </summary>
        public string CodePathWin { get; set; }

        /// <summary>
        /// Gets or sets the general description of what the plugin does. This string is displayed to the user.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the relative path to a PNG image without the .png extension. This image is displayed in the Plugin Store window. The PNG image should be a 72pt x 72pt image. Ideally you should provide a @1x and @2x version of the image. The Stream Deck application takes care of loading the appropriate version of the image.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the name of the plugin.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the relative path to the Property Inspector html file if your plugin want to display some custom settings in the Property Inspector.
        /// </summary>
        public string PropertyInspectorPath { get; set; } = null;

        /// <summary>
        /// Gets or sets the default window size when a Javascript plugin or Property Inspector opens a window using window.open(). Default value is [500, 650].
        /// </summary>
        public int[] DefaultWindowSize { get; set; } = null;

        /// <summary>
        /// Gets or sets a URL displayed to the user if he wants to get more info about the plugin.
        /// </summary>
        public string URL { get; set; } = null;

        /// <summary>
        /// Gets or sets the version of the plugin which can only contain digits and periods. This is used for the software update mechanism.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the SDK version; this value should be set to 2.
        /// </summary>
        public int SDKVersion { get; set; } = 2;

        /// <summary>
        /// Gets or sets which version of the Stream Deck application is required to install the plugin.
        /// </summary>
        [JsonIgnore]
        public string SoftwareMinimumVersion { get; set; } = "4.3";

        /// <summary>
        /// Gets the minimum version of Mac that the plugin requires.
        /// </summary>
        [JsonIgnore]
        public string MacMinimumVersion { get; set; } = null;

        /// <summary>
        /// Gets or sets the applications to monitor on Mac.
        /// </summary>
        [JsonIgnore]
        public string[] MacApplicationsToMonitor { get; set; } = null;

        /// <summary>
        /// Gets the minimum version of Windows that the plugin requires.
        /// </summary>
        [JsonIgnore]
        public string WindowsMinimumVersion { get; set; } = null;

        /// <summary>
        /// Gets or sets the applications to monitor on Windows.
        /// </summary>
        [JsonIgnore]
        public string[] WindowsApplicationsToMonitor { get; set; } = null;
    }
}
