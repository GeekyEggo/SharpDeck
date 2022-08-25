namespace StreamDeck.Extensions
{
    using System.Drawing;

    /// <summary>
    /// Provides extensions methods <see cref="Color"/>.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts the <see cref="Color"/> to a hex representation.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to convert.</param>
        /// <returns>The 6-digit hex representation of the color, e.g. #00ff00.</returns>
        public static string ToHex(this Color color)
            => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}
