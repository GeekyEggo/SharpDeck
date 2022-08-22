namespace SharpDeck.Resources
{
    using System;

    /// <summary>
    /// Provides a static collection of base64 encoded images.
    /// </summary>
    public static class Images
    {
        /// <summary>
        /// Gets the close image.
        /// </summary>
        public static string Close { get; } = GetBase64ImageFromSvg(ImagesManifest.Close);

        /// <summary>
        /// Gets the left arrow image.
        /// </summary>
        public static string Left { get; } = GetBase64ImageFromSvg(ImagesManifest.Left);

        /// <summary>
        /// Gets the an empty image.
        /// </summary>
        public static string None { get; } = GetBase64ImageFromSvg(ImagesManifest.Transparent);

        /// <summary>
        /// Gets the right arrow image.
        /// </summary>
        public static string Right { get; } = GetBase64ImageFromSvg(ImagesManifest.Right);

        /// <summary>
        /// Gets the <see cref="string"/> that represents the base64 encoded <paramref name="image"/> as an SVG image.
        /// </summary>
        /// <param name="image">The image to encode.</param>
        /// <returns>The base64 encoded string.</returns>
        private static string GetBase64ImageFromSvg(byte[] image)
            => $"data:image/svg+xml;base64,{Convert.ToBase64String(image)}";
    }
}
