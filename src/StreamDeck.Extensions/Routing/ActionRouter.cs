namespace StreamDeck.Routing
{
    using System;
    using System.Collections.Concurrent;
    using System.Text.Json.Nodes;
    using Microsoft.Extensions.Logging;
    using StreamDeck;
    using StreamDeck.Events;

    /// <summary>
    /// Provides routing of <see cref="StreamDeckAction"/> in relation to a <see cref="IStreamDeckConnection"/>.
    /// </summary>
    internal class ActionRouter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRouter" /> class.
        /// </summary>
        /// <param name="actionFactory">The action factory, used to create new instances of actions.</param>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="dispatcher">The dispatcher responsible for dispatching propagated event delegates.</param>
        /// <param name="loggerFactory">The optional logger factory.</param>
        public ActionRouter(IActionFactory actionFactory, IDispatcher dispatcher, IStreamDeckConnection connection, ILoggerFactory? loggerFactory = null)
        {
            this.ActionFactory = actionFactory;
            this.Connection = connection;
            this.Dispatcher = dispatcher;
            this.LoggerFactory = loggerFactory;

            connection.WillAppear += this.OnWillAppear;
            connection.WillDisappear += this.OnWillDisappear;

            connection.DidReceiveSettings += this.CreateEventHandler<ActionEventArgs<ActionPayload>>(a => a.OnDidReceiveSettings);
            connection.KeyDown += this.CreateEventHandler<ActionEventArgs<KeyPayload>>(a => a.OnKeyDown);
            connection.KeyUp += this.CreateEventHandler<ActionEventArgs<KeyPayload>>(a => a.OnKeyUp);
            connection.PropertyInspectorDidAppear += this.CreateEventHandler<ActionEventArgs>(a => a.OnPropertyInspectorDidAppear);
            connection.PropertyInspectorDidDisappear += this.CreateEventHandler<ActionEventArgs>(a => a.OnPropertyInspectorDidDisappear);
            connection.SendToPlugin += this.CreateEventHandler<PartialActionEventArgs<JsonObject>>(a => a.OnSendToPlugin);
            connection.TitleParametersDidChange += this.CreateEventHandler<ActionEventArgs<TitlePayload>>(a => a.OnTitleParametersDidChange);
        }

        /// <summary>
        /// Gets the action factory, used to create new instances of actions.
        /// </summary>
        private IActionFactory ActionFactory { get; }

        /// <summary>
        /// Gets the connection with the Stream Deck.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the dispatcher responsible for dispatching propagated event delegates.
        /// </summary>
        private IDispatcher Dispatcher { get; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        private ILoggerFactory? LoggerFactory { get; }

        /// <summary>
        /// Gets the registered routes that map to action types.
        /// </summary>
        private Dictionary<string, Type> Routes { get; } = new Dictionary<string, Type>();

        /// <summary>
        /// Gets the action instances by their context.
        /// </summary>
        private ConcurrentDictionary<string, StreamDeckAction> Actions { get; } = new ConcurrentDictionary<string, StreamDeckAction>();

        /// <summary>
        /// Maps the specified action <paramref name="uuid"/> to the <typeparamref name="TAction"/> type, allowing for <see cref="IStreamDeckConnection"/> events to be routed to an action instance.
        /// </summary>
        /// <typeparam name="TAction">The type of the action to route events to.</typeparam>
        /// <param name="uuid">The the unique identifier of the action; see <see href="https://developer.elgato.com/documentation/stream-deck/sdk/manifest/#actions"/>.</param>
        public void MapAction<TAction>(string uuid)
            where TAction : StreamDeckAction
            => this.Routes.Add(uuid, typeof(TAction));

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillAppear"/>, and attempts to route the event to an action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}"/> instance containing the event data.</param>
        private void OnWillAppear(IStreamDeckConnection sender, ActionEventArgs<ActionPayload> args)
        {
            // Determine if the route is configured.
            if (!this.Routes.TryGetValue(args.Action, out var actionType))
            {
                return;
            }

            // Get or add the action instance for the specified action type.
            var action = this.Actions.GetOrAdd(args.Context, _ =>
            {
                var initializationContext = new ActionInitializationContext(this.Connection, args, this.LoggerFactory?.CreateLogger(actionType));
                return this.ActionFactory.CreateInstance(actionType, initializationContext);
            });

            this.Dispatcher.Invoke(() => action.OnWillAppear(args), args.Context);
        }

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillDisappear"/>, removing and disposing of actions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}"/> instance containing the event data.</param>
        private void OnWillDisappear(IStreamDeckConnection sender, ActionEventArgs<ActionPayload> args)
        {
            // Attempt to remove the action, if successful, invoke OnWillDisappear and dispose of the action.
            if (this.Actions.TryRemove(args.Context, out var action))
            {
                this.Dispatcher.Invoke(async () =>
                {
                    await action.OnWillDisappear(args);
                    action.Dispose();
                }, args.Context);
            }
        }

        /// <summary>
        /// Creates an <see cref="EventHandler"/> that can be attached to one of the events within a <see cref="StreamDeckAction"/>.
        /// </summary>
        /// <typeparam name="TArgs">The type of the arguments supplied to the event handler.</typeparam>
        /// <param name="eventHandler">The event handler to propagate the event onto.</param>
        /// <returns>The event handler.</returns>
        private EventHandler<IStreamDeckConnection, TArgs> CreateEventHandler<TArgs>(Func<StreamDeckAction, Func<TArgs, Task>> eventHandler)
            where TArgs : IActionContext
        {
            return (conn, args) =>
            {
                if (this.Actions.TryGetValue(args.Context, out var action))
                {
                    this.Dispatcher.Invoke(() => eventHandler(action)(args), args.Context);
                }
            };
        }
    }
}
