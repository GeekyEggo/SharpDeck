namespace SharpDeck.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="string"/>.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Gets this instance when it is not <see cref="string.IsNullOrWhiteSpace(string)"/>; otherwise <paramref name="default"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="default">The default value.</param>
        /// <returns><paramref name="str"/> when it is not <see cref="string.IsNullOrWhiteSpace(string)"/>; otherwise <paramref name="default"/>.</returns>
        internal static string OrDefault(this string str, string @default)
            => string.IsNullOrWhiteSpace(str) ? @default : str;
    }
}
