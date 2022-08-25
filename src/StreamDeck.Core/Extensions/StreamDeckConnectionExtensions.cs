namespace StreamDeck.Extensions
{
    using System;
    using System.Drawing;
    using System.Security.Cryptography;

    /// <summary>
    /// Provides extension methods for <see cref="IStreamDeckConnection"/>.
    /// </summary>
    public static class StreamDeckConnectionExtensions
    {
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
            => connection.SetImageAsync(context, SvgToBase64($@"<svg height=""1"" width=""1""><rect width=""1"" height=""1"" fill=""{color.ToHex()}""/></svg>"), target, state, cancellationToken);

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
                return connection.SetImageAsync(context, SvgToBase64(reader.ReadToEnd()), target, state, cancellationToken);
            }
            else if (fileStream.TryGetImageMimeType(out var mimeType))
            {
                using var cryptoStream = new CryptoStream(fileStream, new ToBase64Transform(), CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream);
                return connection.SetImageAsync(context, $"data:{mimeType};base64,{reader.ReadToEnd()}", target, state, cancellationToken);
            }

            throw new NotSupportedException($"Image is not a supported mime type; supported types are BMP, GIF, JPG, PNG, SVG, TIFF.");
        }

        private static string SvgToBase64(string svg)
            => $"data:image/svg+xml;charset=utf8,{svg}";
    }
}
