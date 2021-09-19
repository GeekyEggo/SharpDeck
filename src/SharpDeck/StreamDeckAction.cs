namespace SharpDeck
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;
    using SharpDeck.Interactivity;
    using SharpDeck.PropertyInspectors;

    /// <summary>
    /// Provides a base implementation for a Stream Deck action.
    /// </summary>
    public class StreamDeckAction : StreamDeckButton
    {
        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.GetSettingsAsync(string, CancellationToken)"/> has been called to retrieve the persistent data stored for the action.
        /// </summary>
        private event EventHandler<ActionEventArgs<ActionPayload>> DidReceiveSettings;

        /// <summary>
        /// Gets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        public string ActionUUID { get; private set; }

        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        /// </summary>
        public string Device { get; private set; }

        /// <summary>
        /// Gets or sets the unique identifier assigned by SharpDeck.
        /// </summary>
        internal string SharpDeckUUID { get; set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        internal protected ILogger Logger { get; internal set; }

        /// <summary>
        /// Gets or sets the interval before a long press is invoked; occurs after <see cref="OnKeyDown(ActionEventArgs{KeyPayload})"/>. Setting this to <see cref="TimeSpan.Zero"/> will disable long-press interaction.
        /// </summary>
        protected TimeSpan LongKeyPressInterval { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// Gets the property inspector method collection caches.
        /// </summary>
        private static ConcurrentDictionary<Type, PropertyInspectorMethodCollection> PropertyInspectorMethodCollections { get; } = new ConcurrentDictionary<Type, PropertyInspectorMethodCollection>();

        /// <summary>
        /// Gets the dynamic profile factory.
        /// </summary>
        private IDynamicProfileFactory DynamicProfileFactory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        private bool IsDisposed { get; set; } = false;

        /// <summary>
        /// Gets the stack responsible for monitoring physical key interactions; this is used to determine if the press was a long-press.
        /// </summary>
        private ConcurrentStack<ActionEventArgs<KeyPayload>> KeyPressStack { get; } = new ConcurrentStack<ActionEventArgs<KeyPayload>>();

        /// <summary>
        /// Gets this action's instances settings asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task containing the settings.</returns>
        public Task<T> GetSettingsAsync<T>(CancellationToken cancellationToken = default)
            where T : class
        {
            this.ThrowIfDisposed();

            var taskSource = new TaskCompletionSource<T>();

            // Declare the local function handler that sets the task result.
            void handler(object sender, ActionEventArgs<ActionPayload> e)
            {
                if (taskSource.TrySetResult(e.Payload.GetSettings<T>()))
                {
                    this.DidReceiveSettings -= handler;
                }
            }

            // Register the cancellation.
            cancellationToken.Register(() =>
            {
                if (taskSource.TrySetCanceled())
                {
                    this.DidReceiveSettings -= handler;
                }
            });

            // Listen for receiving events, and trigger a request.
            this.DidReceiveSettings += handler;
            this.Connection.GetSettingsAsync(this.Context);

            return taskSource.Task;
        }

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="payload">A JSON object that will be received by the Property Inspector.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of sending payload to the property inspector.</returns>
        public Task SendToPropertyInspectorAsync(object payload, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();
            return this.Connection.SendToPropertyInspectorAsync(this.Context, this.ActionUUID, payload, cancellationToken);
        }

        /// <summary>
        /// Save persistent data for the actions instance.
        /// </summary>
        /// <param name="settings">A JSON object which is persistently saved for the action's instance.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of setting the settings.</returns>
        public Task SetSettingsAsync(object settings, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();
            return this.Connection.SetSettingsAsync(this.Context, settings, cancellationToken);
        }

        /// <summary>
        /// Sets the context and initializes the action.
        /// </summary>
        /// <param name="args">The arguments containing the context.</param>
        /// <param name="connection">The connection with the Stream Deck responsible for sending and receiving events and messages.</param>
        /// <param name="dynamicProfileFactory">The dynamic profile factory.</param>
        /// <returns>The task of setting the context and initialization.</returns>
        internal void Initialize(ActionEventArgs<AppearancePayload> args, IStreamDeckConnection connection, IDynamicProfileFactory dynamicProfileFactory)
        {
            this.ActionUUID = args.Action;
            this.Context = args.Context;
            this.Device = args.Device;
            this.Connection = connection;

            this.DynamicProfileFactory = dynamicProfileFactory;
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
        /// Occurs when <see cref="IStreamDeckConnection.KeyDown"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        internal protected virtual Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            this.KeyPressStack.Push(args);
            if (this.LongKeyPressInterval > TimeSpan.Zero)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(this.LongKeyPressInterval);
                    this.TryHandleKeyPress(this.OnKeyLongPress);
                });
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.KeyUp"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        internal protected virtual Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            this.TryHandleKeyPress(this.OnKeyPress);
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
        /// Occurs when <see cref="IStreamDeckConnection.TitleParametersDidChange"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{TitlePayload}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        internal protected virtual Task OnTitleParametersDidChange(ActionEventArgs<TitlePayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.WillAppear"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        internal protected virtual Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when <see cref="IStreamDeckConnection.WillDisappear"/> is received for this instance.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}" /> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        internal protected virtual Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            this.TryHandleKeyPress(this.OnKeyPress);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.KeyPressStack.Clear();
            this.Connection = null;

            this.IsDisposed = true;
        }

        /// <summary>
        /// Occurs when this instance is initialized.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        protected virtual void OnInit(ActionEventArgs<AppearancePayload> args) { }

        /// <summary>
        /// When a <see cref="LongKeyPressInterval"/> is defined, this occurs on <see cref="IStreamDeckConnection.KeyUp"/> after the interval has elapsed in relation to <see cref="IStreamDeckConnection.KeyDown"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}"/> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        protected virtual Task OnKeyLongPress(ActionEventArgs<KeyPayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Occurs when the button is pressed, typically on <see cref="IStreamDeckConnection.KeyUp"/>. When a <see cref="LongKeyPressInterval"/> is defined, this will occur on <see cref="IStreamDeckConnection.KeyUp"/> when the interval has not elapsed in relation to <see cref="IStreamDeckConnection.KeyDown"/>.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{KeyPayload}"/> instance containing the event data.</param>
        /// <returns>The task of handling the event.</returns>
        protected virtual Task OnKeyPress(ActionEventArgs<KeyPayload> args)
            => Task.CompletedTask;

        /// <summary>
        /// Shows the collection of <paramref name="items"/> as a dynamic profile asynchronouosly.
        /// </summary>
        /// <typeparam name="TController">The type of the dynamic profile controller.</typeparam>
        /// <typeparam name="TItem">The type of the items the manager is capable of handling.</typeparam>
        /// <param name="items">The items to display in the dynamic profile.</param>
        /// <returns>The result of the dynamic profile.</returns>
        protected async Task<DynamicProfileResult<TItem>> ShowDynamicProfileAsync<TController, TItem>(IEnumerable<TItem> items)
            where TController : class, IDynamicProfileController<TItem>
        {
            try
            {
                return await this.DynamicProfileFactory.Create<TController, TItem>(this.Device)
                    .ShowAsync(items);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, $"Failed to show dynamic profile for action \"{this.ActionUUID}\".");
                await this.ShowAlertAsync();
            }

            return DynamicProfileResult<TItem>.None;
        }

        /// <summary>
        /// Attempts to pop from the <see cref="KeyPressStack"/>; if successful the <paramref name="handler"/> is invoked with the arguments.
        /// </summary>
        /// <param name="handler">The handler.</param>
        private void TryHandleKeyPress(Func<ActionEventArgs<KeyPayload>, Task> handler)
        {
            if (this.KeyPressStack.TryPop(out var args))
            {
                handler(args);
            }
        }

        /// <summary>
        /// Throws the <see cref="ObjectDisposedException"/> if this instance has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException($"{this.ActionUUID}:{this.Context}");
            }
        }
    }
}
