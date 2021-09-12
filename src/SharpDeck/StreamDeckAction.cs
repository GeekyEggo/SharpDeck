namespace SharpDeck
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;
    using SharpDeck.PropertyInspectors;

    /// <summary>
    /// Provides a base implementation for a Stream Deck action.
    /// </summary>
    public class StreamDeckAction : StreamDeckButton, IDisposable
    {
        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.GetSettingsAsync(string)"/> has been called to retrieve the persistent data stored for the action.
        /// </summary>
        private event EventHandler<ActionEventArgs<ActionPayload>> DidReceiveSettings;

        /// <summary>
        /// Gets or sets the unique identifier assigned by SharpDeck.
        /// </summary>
        internal string SharpDeckUUID { get; set; }

        /// <summary>
        /// Gets the property inspector method collection caches.
        /// </summary>
        private static ConcurrentDictionary<Type, PropertyInspectorMethodCollection> PropertyInspectorMethodCollections { get; } = new ConcurrentDictionary<Type, PropertyInspectorMethodCollection>();

        /// <summary>
        /// Gets this action's instances settings asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <returns>The task containing the settings.</returns>
        public Task<T> GetSettingsAsync<T>()
            where T : class
        {
            this.ThrowIfDisposed();

            var taskSource = new TaskCompletionSource<T>();

            // declare the local function handler that sets the task result
            void handler(object sender, ActionEventArgs<ActionPayload> e)
            {
                this.DidReceiveSettings -= handler;
                taskSource.TrySetResult(e.Payload.GetSettings<T>());
            }

            // listen for receiving events, and trigger a request
            this.DidReceiveSettings += handler;
            this.StreamDeck.GetSettingsAsync(this.Context);

            return taskSource.Task;
        }

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        /// <returns>The task of sending payload to the property inspector.</returns>
        public Task SendToPropertyInspectorAsync(object payload)
        {
            this.ThrowIfDisposed();
            return this.StreamDeck.SendToPropertyInspectorAsync(this.Context, this.ActionUUID, payload);
        }

        /// <summary>
        /// Save persistent data for the actions instance.
        /// </summary>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        /// <returns>The task of setting the settings.</returns>
        public Task SetSettingsAsync(object settings)
        {
            this.ThrowIfDisposed();
            return this.StreamDeck.SetSettingsAsync(this.Context, settings);
        }

        /// <summary>
        /// Sets the context and initializes the action.
        /// </summary>
        /// <param name="args">The arguments containing the context.</param>
        /// <param name="connection">The connection with the Stream Deck responsible for sending and receiving events and messages.</param>
        /// <returns>The task of setting the context and initialization.</returns>
        internal void Initialize(ActionEventArgs<AppearancePayload> args, IStreamDeckConnection connection)
        {
            base.Initialize(args, connection);
            this.OnInit(args);
        }

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.DidReceiveSettings"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        internal protected virtual Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            this.DidReceiveSettings?.Invoke(this, args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.PropertyInspectorDidAppear"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        internal protected virtual Task OnPropertyInspectorDidAppear(ActionEventArgs args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.PropertyInspectorDidDisappear"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        internal protected virtual Task OnPropertyInspectorDidDisappear(ActionEventArgs args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.SendToPlugin"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{JObject}"/> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        internal protected virtual async Task OnSendToPlugin(ActionEventArgs<JObject> args)
        {
            try
            {
                var factory = PropertyInspectorMethodCollections.GetOrAdd(this.GetType(), t => new PropertyInspectorMethodCollection(t));
                await factory.InvokeAsync(this, args);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, $"Failed to handle event \"{args.Event}\" for \"{args.Action}\" ({args.Context}).");
                await this.ShowAlertAsync();
            }
        }

        /// <summary>
        /// Occurs when this instance is initialized.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        protected virtual void OnInit(ActionEventArgs<AppearancePayload> args) { }
    }
}
