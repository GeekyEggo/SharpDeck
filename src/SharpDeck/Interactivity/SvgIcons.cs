namespace SharpDeck.Interactivity
{
    using System;

    /// <summary>
    /// Provides a static collection of base64 encoded SVG images.
    /// </summary>
    public static class SvgIcons
    {
        /// <summary>
        /// Gets the transparent SVG.
        /// </summary>
        public static string Transparent { get; } = $"data:image/svg+xml;base64,{Convert.ToBase64String(Resources.Images.Transparent)}";
    }
}
