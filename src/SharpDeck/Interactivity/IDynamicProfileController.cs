namespace SharpDeck.Interactivity
{
    using System.Threading;
    using System.Threading.Tasks;
    using SharpDeck.Enums;

    /// <summary>
    /// Provides methods that support dynamic profiles.
    /// </summary>
    /// <typeparam name="T">The type of the items within the dynamic profile.</typeparam>
    public interface IDynamicProfileController<T>
    {
        /// <summary>
        /// Called when the user selects an item from the dynamic profile.
        /// </summary>
        /// <param name="context">The dynamic profile context; this can be used to set the result of, or close, the dynamic profile.</param>
        /// <param name="item">The item that was selected by the user.</param>
        /// <returns>The task of handling the item being selected.</returns>
        Task OnItemSelectedAsync(DynamicProfileContext<T> context, T item);

        /// <summary>
        /// Called when an item within the dynamic profile is becoming visible; this allows for the button to be customized and styled.
        /// </summary>
        /// <param name="item">The item that has become visible.</param>
        /// <param name="context">The dynamic profile context.</param>
        /// <param name="button">The button that represents the <paramref name="item"/>; this allows for the button to be customized and styled, i.e. the title and image set.</param>
        /// <param name="cancellationToken">The cancellation token; this is cancelled when the current page of the dynamic profile changes, or the dynamic profile is closed.</param>
        /// <returns>The task responsible for rendering the item.</returns>
        Task OnItemWillAppearAsync(DynamicProfileContext<T> context, IButton button, T item, CancellationToken cancellationToken);

        /// <summary>
        /// Tries to get the name of the profile that will be used to display the dynamic profile for the specified <paramref name="deviceType"/>; this profile must be registered within the plugin's manifest.
        /// When the specified <paramref name="deviceType"/> is not supported by this controller, the result is <c>false</c>.
        /// </summary>
        /// <param name="deviceType">The type of the device the dynamic profile is attempting to be displayed on.</param>
        /// <param name="profile">The name of the profile used to display the dynamic profile on the specified <paramref name="deviceType"/>.</param>
        /// <returns><c>true</c> when the <paramref name="deviceType"/> is supported by this controller, and there is a valid profile; otherwise <c>false</c>.</returns>
        bool TryGetProfileName(DeviceType deviceType, out string profile);
    }
}
