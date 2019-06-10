namespace SharpDeck
{
    using SharpDeck.Events.Received;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an implementation of an action, containing settings, that can be registered on a <see cref="StreamDeckClient"/>.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    public class StreamDeckAction<TSettings> : StreamDeckAction
        where TSettings : class
    {
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public virtual TSettings Settings { get; set; }

        /// <summary>
        /// Gets this action's instances settings asynchronously.
        /// </summary>
        /// <returns>The task containing the settings.</returns>
        public Task<TSettings> GetSettingsAsync()
            => this.GetSettingsAsync<TSettings>();

        /// <summary>
        /// Save persistent data for the actions instance.
        /// </summary>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        public Task SetSettingsAsync(TSettings settings)
            => this.StreamDeck.SetSettingsAsync(this.Context, settings);

        /// <summary>
        /// Sets the context and initializes the action.
        /// </summary>
        /// <param name="args">The arguments containing the context.</param>
        /// <param name="streamDeck">The Stream Deck client.</param>
        /// <returns>The task of setting the context and initialization.</returns>
        internal override void Initialize(ActionEventArgs<AppearancePayload> args, IStreamDeckSender streamDeck)
        {
            this.Settings = args.Payload.GetSettings<TSettings>();
            base.Initialize(args, streamDeck);
        }

        /// <summary>
        /// Raises the <see cref="StreamDeckActionEventReceiver.DidReceiveSettings" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        protected async override Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);
            await this.OnDidReceiveSettings(args, args.Payload.GetSettings<TSettings>());
        }

        /// <summary>
        /// A wrapper for <see cref="OnDidReceiveSettings(ActionEventArgs{ActionPayload})" />, with the addition of the strongly typed <paramref name="settings" />.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        /// <param name="settings">The settings.</param>
        protected virtual Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args, TSettings settings)
        {
            this.Settings = settings;
            return Task.CompletedTask;
        }
    }
}
