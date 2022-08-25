namespace StreamDeck.Extensions
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides extension methods for <see cref="Stream"/>.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Supported image mime types, and their bytes that identify them.
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/a/13614746/259656
        /// https://github.com/Muraad/Mime-Detective/blob/master/MimeDetective/MimeTypes.cs
        /// </remarks>
        private static readonly (string MimeType, byte[] Bytes)[] IMAGE_MIME_TYPES = new[]
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            ("image/bmp" , new byte[] { 66, 77 } ),
            ("image/gif", new byte[] { 71, 73, 70, 56} ),
            ("image/jpeg", new byte[] { 255, 216, 255} ),
            ("image/png", new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82} ),
            ("image/tiff", new byte[] { 73, 73, 42, 0} ),
#pragma warning restore IDE0230 // Use UTF-8 string literal
        };

        /// <summary>
        /// Attempts to get the mime type of the <paramref name="stream"/>, when the the mime type is either BMP, GIF, JPG, PNG, or TIFF.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> whose mime-type should be determined.</param>
        /// <param name="mimeType">The mime-type.</param>
        /// <returns><c>true</c> when the mime-type is known image mime-type; otherwise <c>false</c></returns>
        public static bool TryGetImageMimeType(this Stream stream, [NotNullWhen(true)] out string? mimeType)
        {
            mimeType = null;
            if (stream.Length < 2) // BMP is the smallest, at only 2 bytes.
            {
                return false;
            }

            // Remember our current position.
            var position = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);

            var buffer = new byte[16]; // PNG is the biggest at 16 bytes.
            stream.Read(buffer, 0, buffer.Length);

            foreach (var (imgMimeType, bytes) in IMAGE_MIME_TYPES)
            {
                if (buffer.Take(bytes.Length).SequenceEqual(bytes))
                {
                    mimeType = imgMimeType;
                    stream.Seek(position, SeekOrigin.Begin);

                    return true;
                }
            }

            stream.Seek(position, SeekOrigin.Begin);
            return false;
        }
    }

}
