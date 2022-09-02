namespace StreamDeck.Routing
{
    using System;
    using System.Collections.Concurrent;
    using System.Text.Json.Nodes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using StreamDeck;
    using StreamDeck.Events;
    using StreamDeck.Extensions;

    /// <summary>
    /// Provides routing of <see cref="StreamDeckAction"/> in relation to a <see cref="IStreamDeckConnection"/>.
    /// </summary>
    internal class ActionRouter
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRouter" /> class.
        /// </summary>
        /// <param name="connection">The connection..</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="loggerFactory">The optional logger factory.</param>
        public ActionRouter(IStreamDeckConnection connection, IServiceProvider serviceProvider, ILoggerFactory? loggerFactory = null)
        {
            this.Connection = connection;
            this.LoggerFactory = loggerFactory;
            this.ServiceProvider = serviceProvider;

            connection.WillAppear += this.OnWillAppear;
            connection.WillDisappear += this.OnWillDisappear;

            connection.DidReceiveSettings += this.GetDelegateHandler<ActionEventArgs<ActionPayload>>(a => a.OnDidReceiveSettings);
            connection.KeyDown += this.GetDelegateHandler<ActionEventArgs<KeyPayload>>(a => a.OnKeyDown);
            connection.KeyUp += this.GetDelegateHandler<ActionEventArgs<KeyPayload>>(a => a.OnKeyUp);
            connection.PropertyInspectorDidAppear += this.GetDelegateHandler<ActionEventArgs>(a => a.OnPropertyInspectorDidAppear);
            connection.PropertyInspectorDidDisappear += this.GetDelegateHandler<ActionEventArgs>(a => a.OnPropertyInspectorDidDisappear);
            connection.SendToPlugin += this.GetDelegateHandler<PartialActionEventArgs<JsonObject>>(a => a.OnSendToPlugin);
            connection.TitleParametersDidChange += this.GetDelegateHandler<ActionEventArgs<TitlePayload>>(a => a.OnTitleParametersDidChange);
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        private ILoggerFactory? LoggerFactory { get; }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        private IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the registered routes that map to action types.
        /// </summary>
        private Dictionary<string, Type> Routes { get; } = new Dictionary<string, Type>();

        /// <summary>
        /// Gets the action instances by their context.
        /// </summary>
        private Dictionary<string, StreamDeckAction> Actions { get; } = new Dictionary<string, StreamDeckAction>();

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
            using (this._syncRoot.Lock())
            {
                // Determine if the route is configured.
                if (!this.Routes.TryGetValue(args.Action, out var actionType))
                {
                    return;
                }

                // Get or add an action instance for the specified action type.
                if (!this.Actions.TryGetValue(args.Context, out var action))
                {
                    var initializationContext = new ActionInitializationContext(this.Connection, args, this.LoggerFactory?.CreateLogger(actionType));
                    action = (StreamDeckAction)ActivatorUtilities.CreateInstance(this.ServiceProvider, actionType, initializationContext);

                    this.Actions.Add(args.Context, action);
                }

                this.InvokeAsync(args, action.OnWillAppear);
            }
        }

        private ConcurrentDictionary<string, string> Foo = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillDisappear"/>, removing and disposing of actions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ActionEventArgs{ActionPayload}"/> instance containing the event data.</param>
        private void OnWillDisappear(IStreamDeckConnection sender, ActionEventArgs<ActionPayload> args)
        {
            using (this._syncRoot.Lock())
            {
                // Determine if there is an action associated with the context.
                if (!this.Actions.TryGetValue(args.Context, out var action))
                {
                    return;
                }

                this.Actions.Remove(args.Context, out var m);

                // Remove the action, and dispose of it.
                this.Actions.Remove(args.Context);
                this.InvokeAsync(args, async (e) =>
                {
                    await action.OnWillDisappear(e);
                    action.Dispose();
                });
            }
        }

        private EventHandler<IStreamDeckConnection, TArgs> GetDelegateHandler<TArgs>(Func<StreamDeckAction, Func<TArgs, Task>> getPropagator)
            where TArgs : IActionContext
        {
            return (conn, args) =>
            {
                using (this._syncRoot.Lock())
                {
                    if (this.Actions.TryGetValue(args.Context, out var action))
                    {
                        this.InvokeAsync(args, getPropagator(action));
                    }
                }
            };
        }

        private void InvokeAsync<TArgs>(TArgs args, Func<TArgs, Task> method)
            where TArgs : IActionContext
        {
            Task.Factory.StartNew(async (state) =>
            {
                var ctx = (AsyncExecutionContext<TArgs>)state!;

                try
                {
                    await ctx.Method(ctx.Args);
                }
                catch (Exception ex)
                {
                    await this.Connection.ShowAlertAsync(ctx.Args.Context);
                    await this.Connection.LogMessageAsync(ex.Message);
                }
            },
            new AsyncExecutionContext<TArgs>(args, method),
            TaskCreationOptions.RunContinuationsAsynchronously);
        }

        private struct AsyncExecutionContext<TArgs>
        {
            public AsyncExecutionContext(TArgs args, Func<TArgs, Task> method)
            {
                this.Args = args;
                this.Method = method;
            }

            public TArgs Args { get; }
            public Func<TArgs, Task> Method { get; }
        }
    }
}
