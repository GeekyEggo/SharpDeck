namespace StreamDeck
{
    using System.Text.Json.Serialization.Metadata;

    /// <inheritdoc/>
    public partial interface IStreamDeckConnection
    {
        /// <summary>
        /// Requests the persistent global data stored for the plugin.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#getglobalsettings"/>.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of sending the message; this result does not contain the settings.</returns>
        Task GetGlobalSettingsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests the persistent data stored for the specified context's action instance.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#getsettings"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of sending the message; this result does not contain the settings.</returns>
        Task GetSettingsAsync(string context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a debug log to the logs file.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#logmessage"/>.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of logging the message.</returns>
        Task LogMessageAsync(string msg, CancellationToken cancellationToken = default);

        /// <summary>
        /// Open a URL in the default browser.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#openurl"/>.
        /// </summary>
        /// <param name="url">A URL to open in the default browser.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of opening the URL.</returns>
        Task OpenUrlAsync(string url, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#sendtopropertyinspector" />.
        /// </summary>
        /// <typeparam name="TPayload">The type of the payload.</typeparam>
        /// <param name="context">An opaque value identifying the instances action.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        /// <param name="jsonTypeInfo">The optional JSON type information used when serializing the <paramref name="payload" />.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of sending payload to the property inspector.</returns>
        Task SendToPropertyInspectorAsync<TPayload>(string context, string action, TPayload payload, JsonTypeInfo<TPayload>? jsonTypeInfo = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save persistent data for the plugin.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setglobalsettings"/>.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="settings">An object which persistently saved globally.</param>
        /// <param name="jsonTypeInfo">The optional JSON type information used when serializing the <paramref name="settings" />.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the global settings.</returns>
        Task SetGlobalSettingsAsync<TSettings>(TSettings settings, JsonTypeInfo<TSettings>? jsonTypeInfo = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action; starting with Stream Deck 4.5.1, this API accepts svg images.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setimage"/>.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). svg is also supported. If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the image is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the image.</returns>
        Task SetImageAsync(string context, string image = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save persistent data for the actions instance.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setsettings" />.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        /// <param name="jsonTypeInfo">The optional JSON type information used when serializing the <paramref name="settings" />.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the settings.</returns>
        Task SetSettingsAsync<TSettings>(string context, TSettings settings, JsonTypeInfo<TSettings>? jsonTypeInfo = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#settitle"/>.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action you want to modify.</param>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the title.</returns>
        Task SetTitleAsync(string context, string title = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        ///	<see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setstate"/>.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the state.</returns>
        Task SetStateAsync(string context, int state, CancellationToken cancellationToken = default);

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#showalert"/>.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the alert.</returns>
        Task ShowAlertAsync(string context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#showok"/>.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the OK.</returns>
        Task ShowOkAsync(string context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Switch to one of the preconfigured read-only profiles.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#switchtoprofile"/>.
        /// </summary>
        /// <param name="device">An opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.</param>
        /// <param name="profile">The optional name of the profile to switch to. The name should be identical to the name provided in the manifest.json file; when empty, the Stream Deck will switch to the previous profile.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of switching profiles.</returns>
        Task SwitchToProfileAsync(string device, string profile = "", CancellationToken cancellationToken = default);
    }
}
