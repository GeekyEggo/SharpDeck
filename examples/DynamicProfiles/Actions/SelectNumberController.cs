namespace DynamicProfiles.Actions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Enums;
    using SharpDeck.Extensions;
    using SharpDeck.Interactivity;

    /// <summary>
    /// The dynamic profile controller responsible for handling the selection of a number.
    /// </summary>
    public class SelectNumberController : IDynamicProfileController<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectNumberController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SelectNumberController(ILogger<SelectNumberController> logger)
            => this.Logger = logger;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger Logger { get; }

        /// <inheritdoc/>
        public Task OnItemSelectedAsync(DynamicProfileContext<int> context, int item)
        {
            this.Logger.LogTrace($"Selected {item}.");

            context.Profile.CloseWithResult(item);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task OnItemWillAppearAsync(DynamicProfileContext<int> context, IButton button, int item, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await button.SetImageAsync(cancellationToken: cancellationToken);
                await button.SetTitleAsync(item.ToString(), cancellationToken: cancellationToken);
            }
        }

        /// <inheritdoc/>
        public bool TryGetProfileName(DeviceType deviceType, out string profile)
        {
            if (deviceType == DeviceType.StreamDeckXL)
            {
                profile = deviceType.GetProfileName("Profiles/DynamicProfile");
                return true;
            }

            profile = string.Empty;
            return false;
        }
    }
}
