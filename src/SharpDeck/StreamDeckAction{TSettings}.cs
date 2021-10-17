namespace SharpDeck
{
    using System.Threading.Tasks;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides a base implementation for a Stream Deck action that contains custom settings.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    public class StreamDeckAction<TSettings> : StreamDeckAction
        where TSettings : class
    {
        /// <summary>
        /// Gets this action's instances settings asynchronously.
        /// </summary>
        /// <returns>The task containing the settings.</returns>
        public Task<TSettings> GetSettingsAsync()
            => this.GetSettingsAsync<TSettings>();

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.DidReceiveSettings"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        protected internal override Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
            => this.OnDidReceiveSettings(args, args.Payload.GetSettings<TSettings>());

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.DidReceiveSettings"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The task of handling the event.</returns>
        protected virtual Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args, TSettings settings)
            => base.OnDidReceiveSettings(args);

        /// <summary>
        /// Occurs when this instance is initialized.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}" /> instance containing the event data.</param>
        protected override void OnInit(ActionEventArgs<AppearancePayload> args)
            => this.OnInit(args, args.Payload.GetSettings<TSettings>());

        /// <summary>
        /// Occurs when this instance is initialized.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        /// <param name="settings">The settings.</param>
        protected virtual void OnInit(ActionEventArgs<AppearancePayload> args, TSettings settings) { }
    }
}
