namespace SharpDeck.Interactivity
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Enums;

    /// <summary>
    /// Provides methods that enable rendering and feedback to be displayed on a Stream Deck button.
    /// </summary>
    public interface IButton : IDisposable
    {
        /// <summary>
        /// Gets an opaque value identifying the instance of the action. You will need to pass this opaque value to several APIs like the `setTitle` API.
        /// </summary>
        string Context { get; }

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action; starting with Stream Deck 4.5.1, this API accepts svg images.
        /// </summary>
        /// <param name="image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). svg is also supported. If no image is passed, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the image is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the image.</returns>
        Task SetImageAsync(string image = "", TargetType target = TargetType.Both, int? state = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///	Change the state of the actions instance supporting multiple states.
        /// </summary>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the state.</returns>
        Task SetStateAsync(int state = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="title">The title to display. If no title is passed, the title is reset to the default title from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the title.</returns>
        Task SetTitleAsync(string title = "", TargetType target = TargetType.Both, int? state = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the alert.</returns>
        Task ShowAlertAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of showing the OK.</returns>
        Task ShowOkAsync(CancellationToken cancellationToken = default);
    }
}
