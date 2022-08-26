namespace StreamDeck.Extensions
{
    using System;
    using System.Drawing;
    using System.Security.Cryptography;
    using System.Text.Json.Serialization.Metadata;
    using System.Threading;
    using StreamDeck.Events;

    /// <summary>
    /// Provides extension methods for <see cref="IStreamDeckConnection"/>.
    /// </summary>
    public static class StreamDeckConnectionExtensions
    {
        /// <summary>
        /// Requests the persistent global data stored for the plugin, and awaits a response from the Stream Deck.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#getglobalsettings"/>.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceiveglobalsettings"/>.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="connection">The <see cref="IStreamDeckConnection"/> where the settings are being requested from.</param>
        /// <param name="jsonTypeInfo">The optional JSON type information used when deserializing the settings.</param>
        /// <param name="cancellationToken">The optional cancellation token used to cancel the request.</param>
        /// <returns>The global settings.</returns>
        public static async Task<TSettings?> GetGlobalSettingsAsync<TSettings>(this IStreamDeckConnection connection, JsonTypeInfo<TSettings>? jsonTypeInfo = null, CancellationToken cancellationToken = default)
            where TSettings : class
        {
            var tcs = new TaskCompletionSource<TSettings?>();
            void handler(object sender, StreamDeckEventArgs<SettingsPayload> e)
            {
                if (tcs.TrySetResult(e.Payload.GetSettings(jsonTypeInfo)))
                {
                    connection.DidReceiveGlobalSettings -= handler;
                }
            }

            try
            {
                cancellationToken.Register(t => ((TaskCompletionSource<TSettings?>)t).TrySetCanceled(), tcs, useSynchronizationContext: false);

                connection.DidReceiveGlobalSettings += handler;
                await connection.GetGlobalSettingsAsync(cancellationToken);

                return await tcs.Task;
            }
            finally
            {
                connection.DidReceiveGlobalSettings -= handler;
            }
        }

        /// <summary>
        /// Requests the persistent data stored for the specified context's action instance.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#getsettings"/>.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-received/#didreceivesettings"/>.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="connection">The <see cref="IStreamDeckConnection"/> where the settings are being requested from.</param>
        /// <param name="context">The action context whose settings should be retrieved.</param>
        /// <param name="jsonTypeInfo">The optional JSON type information used when deserializing the settings.</param>
        /// <param name="cancellationToken">The optional cancellation token used to cancel the request.</param>
        /// <returns>The action's settings.</returns>
        public static async Task<TSettings?> GetSettingsAsync<TSettings>(this IStreamDeckConnection connection, string context, JsonTypeInfo<TSettings>? jsonTypeInfo = null, CancellationToken cancellationToken = default)
            where TSettings : class
        {
            var tcs = new TaskCompletionSource<TSettings?>();
            void handler(object sender, ActionEventArgs<ActionPayload> e)
            {
                if (e.Context == context
                    && tcs.TrySetResult(e.Payload.GetSettings(jsonTypeInfo)))
                {
                    connection.DidReceiveSettings -= handler;
                }
            }

            try
            {
                cancellationToken.Register(t => ((TaskCompletionSource<TSettings?>)t).TrySetCanceled(), tcs, useSynchronizationContext: false);

                connection.DidReceiveSettings += handler;
                await connection.GetSettingsAsync(context, cancellationToken);

                return await tcs.Task;
            }
            finally
            {
                connection.DidReceiveSettings -= handler;
            }
        }

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setimage"/>.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="color">The color of the image to display.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the image is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the image.</returns>
        public static Task SetImageFromColorAsync(this IStreamDeckConnection connection, string context, Color color, Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
            => connection.SetImageAsync(context, $@"data:image/svg+xml;charset=utf8,<svg height=""1"" width=""1""><rect width=""1"" height=""1"" fill=""{color.ToHex()}""/></svg>", target, state, cancellationToken);

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// <see href="https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setimage"/>.
        /// </summary>
        /// <param name="context">An opaque value identifying the instance's action.</param>
        /// <param name="path">The path of the image to display; supports BMP, GIF, JPEG, PNG, SVG, and TIFF images.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the image is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the image.</returns>
        public static Task SetImageFromFileAsync(this IStreamDeckConnection connection, string context, string path, Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
        {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            if (path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                using var reader = new StreamReader(fileStream);
                return connection.SetImageAsync(context, $"data:image/svg+xml;charset=utf8,{reader.ReadToEnd()}", target, state, cancellationToken);
            }
            else if (fileStream.TryGetImageMimeType(out var mimeType))
            {
                using var cryptoStream = new CryptoStream(fileStream, new ToBase64Transform(), CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream);
                return connection.SetImageAsync(context, $"data:{mimeType};base64,{reader.ReadToEnd()}", target, state, cancellationToken);
            }

            throw new NotSupportedException($"Image is not a supported mime type; supported types are BMP, GIF, JPG, PNG, SVG, TIFF.");
        }
    }
}
