namespace StreamDeck.Extensions
{
    using System;
    using System.Text.Json.Serialization.Metadata;

    /// <summary>
    /// Provides extension methods for <see cref="StreamDeckAction"/>.
    /// </summary>
    public static class StreamDeckActionExtensions
    {
        /// <summary>
        /// Requests the persistent data stored for the specified context's action instance.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#getsettings"/>.
        /// </summary>
        /// <param name="action">The <see cref="StreamDeckAction"/>.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of sending the message; this result does not contain the settings.</returns>
        public static Task GetSettingsAsync(this StreamDeckAction action, CancellationToken cancellationToken = default)
        {
            ThrowIfNotInitialized(action);
            return action.Connection!.GetSettingsAsync(action.Context!, cancellationToken);
        }

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#sendtopropertyinspector" />.
        /// </summary>
        /// <typeparam name="TPayload">The type of the payload.</typeparam>
        /// <param name="action">The <see cref="StreamDeckAction"/>.</param>
        /// <param name="action">The action unique identifier.</param>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        /// <param name="jsonTypeInfo">The optional JSON type information used when serializing the <paramref name="payload" />.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of sending payload to the property inspector.</returns>
        public static Task SendToPropertyInspectorAsync<TPayload>(this StreamDeckAction action, TPayload payload, JsonTypeInfo<TPayload>? jsonTypeInfo = null, CancellationToken cancellationToken = default)
        {
            ThrowIfNotInitialized(action);
            return action.Connection!.SendToPropertyInspectorAsync(action.Context!, action.ActionUUID!, payload, jsonTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action; starting with Stream Deck 4.5.1, this API accepts svg images.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setimage"/>.
        /// </summary>
        /// <param name="action">The <see cref="StreamDeckAction"/>.</param>
        /// <param name="image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). svg is also supported. If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the image is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the image.</returns>
        public static Task SetImageAsync(this StreamDeckAction action, string image = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
        {
            ThrowIfNotInitialized(action);
            return action.Connection!.SetImageAsync(action.Context!, image, target, state, cancellationToken);
        }

        /// <summary>
        /// Save persistent data for the actions instance.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setsettings" />.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="action">The <see cref="StreamDeckAction"/>.</param>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        /// <param name="jsonTypeInfo">The optional JSON type information used when serializing the <paramref name="settings" />.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the settings.</returns>
        public static Task SetSettingsAsync<TSettings>(this StreamDeckAction action, TSettings settings, JsonTypeInfo<TSettings>? jsonTypeInfo = null, CancellationToken cancellationToken = default)
            where TSettings : class
        {
            ThrowIfNotInitialized(action);
            return action.Connection!.SetSettingsAsync(action.Context!, settings, jsonTypeInfo, cancellationToken);
        }

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        ///	<see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setstate"/>.
        /// </summary>
        /// <param name="action">The <see cref="StreamDeckAction"/>.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the state.</returns>
        public static Task SetStateAsync(this StreamDeckAction action, int state, CancellationToken cancellationToken = default)
        {
            ThrowIfNotInitialized(action);
            return action.Connection!.SetStateAsync(action.Context!, state, cancellationToken);
        }

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#showalert"/>.
        /// </summary>
        /// <param name="action">The <see cref="StreamDeckAction"/>.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the alert.</returns>
        public static Task ShowAlertAsync(this StreamDeckAction action, CancellationToken cancellationToken = default)
        {
            ThrowIfNotInitialized(action);
            return action.Connection!.ShowAlertAsync(action.Context!, cancellationToken);
        }

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#showok"/>.
        /// </summary>
        /// <param name="action">The <see cref="StreamDeckAction"/>.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the OK.</returns>
        public static Task ShowOkAsync(this StreamDeckAction action, CancellationToken cancellationToken = default)
        {
            ThrowIfNotInitialized(action);
            return action.Connection!.ShowOkAsync(action.Context!, cancellationToken);
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if the <see cref="StreamDeckAction"/> is not able to communicate with the Stream Deck.
        /// </summary>
        /// <param name="action">The action to valid.</param>
        private static void ThrowIfNotInitialized(StreamDeckAction action)
        {
            if (action.ActionUUID == null
                || action.Context == null
                || action.Connection == null)
            {
                throw new InvalidOperationException("Unable to communicate with Stream Deck before the action has been initialized.");
            }
        }
    }
}
