namespace SharpDeck.Interactivity
{
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Enums;

    /// <summary>
    /// Provides methods that support drill downs.
    /// </summary>
    /// <typeparam name="T">The type of the items within the drill down.</typeparam>
    public interface IDrillDownController<T>
    {
        /// <summary>
        /// Called when the user selects an item from the drill down.
        /// </summary>
        /// <param name="context">The drill down context; this can be used to set the result of, or close, the drill down.</param>
        /// <param name="item">The item that was selected by the user.</param>
        /// <returns>The task of handling the item being selected.</returns>
        Task OnSelectedAsync(DrillDownContext<T> context, T item);

        /// <summary>
        /// Called when an item within the drill down becomes visible; this allows for the button to be customized and styled via the <paramref name="button"/>.
        /// </summary>
        /// <param name="item">The item that has become visible.</param>
        /// <param name="context">The drill down context.</param>
        /// <param name="button">The button that represents the <paramref name="item"/>; this allows for the button to be customized and styled, i.e. the title and image set.</param>
        /// <param name="cancellationToken">The cancellation token; this is cancelled when the current page of the drill down changes, or the drill down is closed.</param>
        /// <returns>The task responsible for rendering the item.</returns>
        Task OnShowAsync(DrillDownContext<T> context, IButton button, T item, CancellationToken cancellationToken);

        /// <summary>
        /// Tries to get the name of the profile that will be used to display the drill down for the specified <paramref name="deviceType"/>; this profile must be registered within the plugin's manifest.
        /// When the specified <paramref name="deviceType"/> is not supported by this controller, the result is <c>false</c>.
        /// </summary>
        /// <param name="deviceType">The type of the device the drill down is attempting to be displayed on.</param>
        /// <param name="profile">The name of the profile used to display the drill down on the specified <paramref name="deviceType"/>.</param>
        /// <returns><c>true</c> when the <paramref name="deviceType"/> is supported by this controller, and there is a valid profile; otherwise <c>false</c>.</returns>
        bool TryGetProfileName(DeviceType deviceType, out string profile);
    }
}
