namespace SharpDeck.Extensions
{
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Interactivity;

    /// <summary>
    /// Provides extension methods for <see cref="IButton"/>.
    /// </summary>
    internal static class ButtonExtensions
    {
        /// <summary>
        /// Sets the title and image of a button asynchronous.
        /// </summary>
        /// <param name="button">The button to update; this instance.</param>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). svg is also supported. If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task of setting the title and image of the button.</returns>
        public static Task SetDisplayAsync(this IButton button, string title = "", string image = "", CancellationToken cancellationToken = default)
        {
            return button == null
                ? Task.CompletedTask
                : Task.WhenAll(
                    button.SetTitleAsync(title, cancellationToken: cancellationToken),
                    button.SetImageAsync(image, cancellationToken: cancellationToken));
        }
    }
}
