namespace SharpDeck
{
    using Enums;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides method for communication with an Elgato Stream Deck.
    /// </summary>
    public interface IStreamDeckSender
    {
        /// <summary>
        /// Requests the persistent global data stored for the plugin.
        /// </summary>
        /// <returns>The task of sending the message; this result does not contain the settings.</returns>
        Task GetGlobalSettingsAsync();

        /// <summary>
        /// Requests the persistent data stored for the specified context's action instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The task of sending the message; this result does not contain the settings.</returns>
        Task GetSettingsAsync(string context);

        /// <summary>
        /// Write a debug log to the logs file.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        Task LogMessageAsync(string msg);

        /// <summary>
        /// Open a URL in the default browser.
        /// </summary>
        /// <param name="url">A URL to open in the default browser.</param>
        Task OpenUrlAsync(string url);

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="context">An opaque value identifying the instances action.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        Task SendToPropertyInspectorAsync(string context, string action, object payload);

        /// <summary>
        /// Save persistent data for the plugin.
        /// </summary>
        /// <param name="settings">An object which persistently saved globally.</param>
        Task SetGlobalSettingsAsync(object settings);

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="base64Image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        Task SetImageAsync(string context, string base64Image, TargetType target = TargetType.Both);

        /// <summary>
        /// Save persistent data for the actions instance.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        Task SetSettingsAsync(string context, object settings);

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action you want to modify.</param>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        Task SetTitleAsync(string context, string title = "", TargetType target = TargetType.Both);

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        Task SetStateAsync(string context, int state = 0);

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        Task ShowAlertAsync(string context);

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        Task ShowOkAsync(string context);

        /// <summary>
        /// Switch to one of the preconfigured read-only profiles.
        /// </summary>
        /// <param name="context">An opaque value identifying the plugin. This value should be set to the PluginUUID received during the registration procedure.</param>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="profile">The name of the profile to switch to. The name should be identical to the name provided in the manifest.json file.</param>
        Task SwitchToProfileAsync(string context, string device, string profile);
    }
}
