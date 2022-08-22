namespace SharpDeck.Connectivity
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Events.Received;
    using SharpDeck.Extensions;
    using SharpDeck.Interactivity;

    /// <summary>
    /// Provides a registry of actions to be handled by the plugin.
    /// </summary>
    internal sealed class StreamDeckActionRegistry : IStreamDeckActionRegistry
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckActionRegistry" /> class.
        /// </summary>
        /// <param name="connection">The connection with the Stream Deck responsible for sending and receiving events and messages.</param>
        /// <param name="dynamicProfileFactory">The dynamic profile factory.</param>
        /// <param name="serviceProvider">The service provider responsible for creating <see cref="StreamDeckAction" />.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public StreamDeckActionRegistry(IStreamDeckConnection connection, IDynamicProfileFactory dynamicProfileFactory, IServiceProvider serviceProvider, ILoggerFactory loggerFactory = null)
        {
            this.Cache = new StreamDeckActionCacheCollection(connection);
            this.Connection = connection;
            this.DynamicProfileFactory = dynamicProfileFactory;
            this.Logger = loggerFactory?.CreateLogger<StreamDeckActionRegistry>();
            this.LoggerFactory = loggerFactory;
            this.ServiceProvider = serviceProvider;

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
        /// Gets or sets a value indicating whether this registry is enabled.
        /// </summary>
        internal bool IsEnabled { get; set; } = false;

        /// <summary>
        /// Gets the actions that have been initialized, and can be invoked when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private IStreamDeckActionCacheCollection Cache { get; }

        /// <summary>
        /// Gets the connection with the Stream Deck responsible for sending and receiving events and messages.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the dynamic profile factory.
        /// </summary>
        private IDynamicProfileFactory DynamicProfileFactory { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        private ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the registered actions, used to initialize new instances of actions.
        /// </summary>
        private IDictionary<string, Type> RegisteredActions { get; } = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Gets service provider responsible for creating <see cref="StreamDeckAction" />.
        /// </summary>
        private IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Registers the specified <typeparamref name="T"/> action, with the given <paramref name="actionUUID"/>.
        /// </summary>
        /// <typeparam name="T">The type of the action.</typeparam>
        /// <param name="actionUUID">The action UUID.</param>
        /// <returns>The modified registry.</returns>
        public IStreamDeckActionRegistry Register<T>(string actionUUID)
            where T : StreamDeckAction
        {
            this.RegisteredActions.Add(actionUUID, typeof(T));
            return this;
        }

        /// <summary>
        /// Registers all instances of <see cref="StreamDeckAction" /> with the specified <paramref name="assembly" />; the action UUID is determined by the <see cref="StreamDeckActionAttribute.UUID" />
        /// </summary>
        /// <param name="assembly">The assembly to search in.</param>
        /// <returns>The modified registry.</returns>
        public IStreamDeckActionRegistry RegisterAll(Assembly assembly)
        {
            foreach (var (type, attr) in assembly.GetTypesWithCustomAttribute<StreamDeckActionAttribute>())
            {
                if (typeof(StreamDeckAction).IsAssignableFrom(type))
                {
                    // The attribute is on a valid attribute.
                    this.RegisteredActions.Add(attr.UUID, type);
                }
                else
                {
                    // The attribute is not on a supported type.
                    throw new NotSupportedException($"Failed to register \"{attr.UUID}\"; class must inherit \"{typeof(StreamDeckAction)}\" or \"{typeof(StreamDeckAction<>)}\".");
                }
            }

            return this;
        }

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillAppear"/> event of the <see cref="Connection"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private void Action_WillAppear(object sender, ActionEventArgs<AppearancePayload> args)
        {
            // Check if the action type is handled by this instance.
            if (this.IsEnabled
                && this.RegisteredActions.TryGetValue(args.Action, out var type))
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
                    action = (StreamDeckAction)ActivatorUtilities.CreateInstance(this.ServiceProvider, actionType);
                    action.Logger = this.LoggerFactory?.CreateLogger(actionType);

                    await this.Cache.AddAsync(args, action);
                    action.Initialize(args, this.Connection, this.DynamicProfileFactory);
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
            if (this.IsEnabled
                && this.Cache.TryGet(args, out var action))
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
