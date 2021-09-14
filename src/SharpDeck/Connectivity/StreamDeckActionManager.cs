namespace SharpDeck.Connectivity
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SharpDeck.DependencyInjection;
    using SharpDeck.Events.Received;
    using SharpDeck.Interactivity;

    /// <summary>
    /// Provides connectivity between a <see cref="IStreamDeckConnection"/> and registered <see cref="StreamDeckAction"/>.
    /// </summary>
    internal sealed class StreamDeckActionManager : IStreamDeckActionManager
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionManager" /> class.
        /// </summary>
        /// <param name="connection">The connection with the Stream Deck responsible for sending and receiving events and messages.</param>
        /// <param name="activator">The activator responsible for creating <see cref="StreamDeckAction" />.</param>
        /// <param name="drillDownFactory">The drill-down factory.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public StreamDeckActionManager(IStreamDeckConnection connection, IActivator activator, IDrillDownFactory drillDownFactory, ILoggerFactory loggerFactory = null)
        {
            this.Activator = activator;
            this.Cache = new StreamDeckActionCacheCollection(connection);
            this.Connection = connection;
            this.DrillDownFactory = drillDownFactory;
            this.Logger = loggerFactory?.CreateLogger<StreamDeckActionManager>();
            this.LoggerFactory = loggerFactory;

            // responsible for caching
            connection.WillAppear += this.Action_WillAppear;

            // action propagation
            connection.DidReceiveSettings               += (_, e) => this.InvokeOnAction(e, a => a.OnDidReceiveSettings);
            connection.KeyDown                          += (_, e) => this.InvokeOnAction(e, a => a.OnKeyDown);
            connection.KeyUp                            += (_, e) => this.InvokeOnAction(e, a => a.OnKeyUp);
            connection.PropertyInspectorDidAppear       += (_, e) => this.InvokeOnAction(e, a => a.OnPropertyInspectorDidAppear);
            connection.PropertyInspectorDidDisappear    += (_, e) => this.InvokeOnAction(e, a => a.OnPropertyInspectorDidDisappear);
            connection.SendToPlugin                     += (_, e) => this.InvokeOnAction(e, a => a.OnSendToPlugin);
            connection.TitleParametersDidChange         += (_, e) => this.InvokeOnAction(e, a => a.OnTitleParametersDidChange);
            connection.WillDisappear                    += (_, e) => this.InvokeOnAction(e, a => a.OnWillDisappear);
        }

        /// <summary>
        /// Gets the activator responsible for creating <see cref="StreamDeckAction" />
        /// </summary>
        private IActivator Activator { get; }

        /// <summary>
        /// Gets the actions that have been initialized, and can be invoked when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private IStreamDeckActionCacheCollection Cache { get; }

        /// <summary>
        /// Gets the connection with the Stream Deck responsible for sending and receiving events and messages.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the drill-down factory.
        /// </summary>
        private IDrillDownFactory DrillDownFactory { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        private ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the registered actions, used to initialize new instances of actions.
        /// </summary>
        private IDictionary<string, Type> RegisteredActions { get; } = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Registers a new <see cref="StreamDeckAction" /> for the specified action UUID.
        /// </summary>
        /// <param name="actionUUID">The action UUID associated with the Stream Deck.</param>
        /// <param name="type">The underlying type of the <see cref="StreamDeckAction"/>.</param>
        public void Register(string actionUUID, Type type)
            => this.RegisteredActions.Add(actionUUID, type);

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillAppear"/> event of the <see cref="Connection"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private void Action_WillAppear(object sender, ActionEventArgs<AppearancePayload> args)
        {
            // Check if the action type is handled by this instance.
            if (this.RegisteredActions.TryGetValue(args.Action, out var type))
            {
                _ = this.Action_WillAppearAsync(args, type);
            }
        }

        /// <summary>
        /// Handles initializing the action and invoking the <see cref="IStreamDeckConnection.WillAppear" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}" /> instance containing the event data.</param>
        /// <param name="actionType">Type of the action.</param>
        private async Task Action_WillAppearAsync(ActionEventArgs<AppearancePayload> args, Type actionType)
        {
            try
            {
                await _syncRoot.WaitAsync();

                if (!this.Cache.TryGet(args, out var action, args.Payload))
                {
                    action = (StreamDeckAction)this.Activator.CreateInstance(actionType);
                    action.Logger = this.LoggerFactory?.CreateLogger(actionType);

                    await this.Cache.AddAsync(args, action);
                    action.Initialize(args, this.Connection, this.DrillDownFactory);
                }

                _ = this.InvokeOnActionAsync(action, args, a => a.OnWillAppear);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, $"Failed to load action \"{args.Action}\".");

                await this.Connection.ShowAlertAsync(args.Context);
                await this.Connection.LogMessageAsync(ex.Message);
            }
            finally
            {
                _syncRoot.Release();
            }
        }

        /// <summary>
        /// Attempts to invoke the event on its associated instance, using <paramref name="getPropagator"/> to determine the delegate that should be invoked.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments parameters.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <param name="getPropagator">The selector to get the propagation event.</param>
        private void InvokeOnAction<T>(T args, Func<StreamDeckAction, Func<T, Task>> getPropagator)
            where T : IActionEventArgs
        {
            // Invokes the event on the action.
            if (this.Cache.TryGet(args, out var action))
            {
                _ = this.InvokeOnActionAsync(action, args, getPropagator);
            }
        }

        /// <summary>
        /// Attempts to propagate the event on an associated action, if it exists within the cache.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments parameters.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="getPropagator">The selector to get the propagation event.</param>
        /// <returns>The task of invoking the event on the action.</returns>
        private Task InvokeOnActionAsync<T>(StreamDeckAction action, T args, Func<StreamDeckAction, Func<T, Task>> getPropagator)
            where T : IActionEventArgs
        {
            return Task.Run(async () =>
            {
                try
                {
                    // We dont want the main thread to await the invocation, but we do want to maintain a context to enable error capturing.
                    await getPropagator(action)(args);
                }
                catch (Exception ex)
                {
                    await this.Connection.LogMessageAsync(ex.Message);
                    await this.Connection.ShowAlertAsync(action.Context);
                }
            });
        }
    }
}
