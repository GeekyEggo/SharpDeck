namespace SharpDeck.Events.Received
{
    using Extensions;
    using Microsoft.Extensions.Logging;
    using Net;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Exceptions;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// A factory that provides information about supported events received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventRouter : IDisposable
    {
        /// <summary>
        /// Gets the event method cache, providing the delegates for each event triggered by Stream Deck.
        /// </summary>
        private static Lazy<IReadOnlyDictionary<string, MethodInfo>> EventMethodCache { get; } = new Lazy<IReadOnlyDictionary<string, MethodInfo>>(GetEventMethodCache, true);

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckEventRouter"/> class.
        /// </summary>
        /// <param name="logger">The optional logger.</param>
        public StreamDeckEventRouter(ILogger logger = null)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the actions factory, used to initialize new instances of actions.
        /// </summary>
        private IDictionary<string, Func<ActionEventArgs<AppearancePayload>, StreamDeckAction>> ActionFactory { get; } = new Dictionary<string, Func<ActionEventArgs<AppearancePayload>, StreamDeckAction>>();

        /// <summary>
        /// Gets the actions that have been initialized, and can be invoked when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private ConcurrentDictionary<string, StreamDeckAction> Actions { get; } = new ConcurrentDictionary<string, StreamDeckAction>();

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var action in this.Actions)
            {
                action.Value.Dispose();
            }

            this.Actions.Clear();
        }

        /// <summary>
        /// Registers a new <see cref="StreamDeckAction"/> for the specified action UUID.
        /// </summary>
        /// <typeparam name="T">The type of Stream Deck action.</typeparam>
        /// <param name="actionUUID">The action UUID.</param>
        /// <param name="valueFactory">The value factory, used to initialize a new action.</param>
        public void Register<T>(string actionUUID, Func<ActionEventArgs<AppearancePayload>, T> valueFactory)
            where T : StreamDeckAction
            => this.ActionFactory.Add(actionUUID, valueFactory);

        /// <summary>
        /// Routes the message specified within the event arguments, invoking any associated delegates where possible.
        /// </summary>
        /// <param name="client">The Stream Deck client.</param>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs" /> instance containing the event data.</param>
        public Task RouteAsync(StreamDeckClient client, WebSocketMessageEventArgs e)
        {
            // determine if there is an event specified, and a delegate handler
            var jArgs = JObject.Parse(e.Message);
            if (!jArgs.TryGetString(nameof(StreamDeckEventArgs.Event), out var @event)
                || !EventMethodCache.Value.TryGetValue(@event, out var @delegate))
            {
                this.Logger?.LogWarning("Unrecognised event received from Elgato Stream Deck: {0}", @event);
                return Task.CompletedTask;
            }

            // determine the owner of the delegate, based on the event args
            var owner = this.TryGetActionInfo(jArgs, out var actionInfo)
                ? this.GetActionOrClient(jArgs, actionInfo, client)
                : client;

            try
            {
                return (Task)@delegate.Invoke(owner, new[] { jArgs.ToObject(@delegate.GetParameters()[0].ParameterType) });
            }
            catch (Exception ex)
            {
                throw new ActionInvokeException(actionInfo.context, ex);
            }
        }

        /// <summary>
        /// Tries to get the action information based on the <paramref name="jArgs"/>.
        /// </summary>
        /// <param name="jArgs">The arguments.</param>
        /// <param name="info">The action information.</param>
        /// <returns><c>true</c> when action information was parsed from the arguments; otherwise <c>false</c>.</returns>
        private bool TryGetActionInfo(JObject jArgs, out (string actionUUID, string context, string device) info)
        {
            // attempt to get the device, action identifier and context
            jArgs.TryGetString(nameof(ActionEventArgs<object>.Device), out info.device);

            return jArgs.TryGetString(nameof(ActionEventArgs<object>.Action), out info.actionUUID)
                & jArgs.TryGetString(nameof(ActionEventArgs<object>.Context), out info.context);
        }

        /// <summary>
        /// Gets the <see cref="StreamDeckAction"/> for the specified parameters; otherwise the default is returned.
        /// </summary>
        /// <param name="jArgs">The original arguments from the Stream Deck event.</param>
        /// <param name="info">The action information.</param>
        /// <param name="client">The Stream Deck client.</param>
        /// <returns>The action instance, a new instance of an action, or the default.</returns>
        private IStreamDeckActionReceiver GetActionOrClient(JObject jArgs, (string actionUUID, string context, string device) info, StreamDeckClient client)
        {
            // when there is no registered action for the action UUID, return the default handler
            if (!this.ActionFactory.TryGetValue(info.actionUUID, out var valueFactory))
            {
                return client;
            }

            // otherwise attempt to get the instance of the action
            return this.Actions.GetOrAdd(info.context, _ =>
            {
                var args = jArgs.ToObject<ActionEventArgs<AppearancePayload>>();
                var action = valueFactory(args);
                action.Initialize(args, client);

                return action;
            });
        }

        /// <summary>
        /// Gets the event method cache containing a dictionary of event names, as triggered by Stream Deck, as their associated delegate.
        /// </summary>
        /// <returns>The event method cache.</returns>
        private static IReadOnlyDictionary<string, MethodInfo> GetEventMethodCache()
        {
            var cache = new Dictionary<string, MethodInfo>();
            foreach (var methodInfo in typeof(StreamDeckClient).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                // only add methods that have the attribute, accept a single parameter, and return a task
                var attr = methodInfo.GetCustomAttribute<StreamDeckEventAttribute>();
                if (attr != null && methodInfo.GetParameters().Length == 1 && methodInfo.ReturnType == typeof(Task))
                {
                    cache.Add(attr.Event, methodInfo);
                }
            }

            return cache;
        }
    }
}
