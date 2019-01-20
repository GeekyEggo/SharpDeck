namespace SharpDeck.Events
{
    using Net;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// A factory that provides information about supported events received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventRouter
    {
        /// <summary>
        /// Initializes the static members of the <see cref="StreamDeckEventRouter"/> class.
        /// </summary>
        static StreamDeckEventRouter()
        {
            // construct the delegate map
            var delegateMap = new Dictionary<string, StreamDeckEventInfo>();
            foreach (var method in typeof(StreamDeckClient).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                var attr = method.GetCustomAttribute<StreamDeckEventAttribute>();
                if (attr != null)
                {
                    delegateMap.Add(attr.Name, new StreamDeckEventInfo(attr, method));
                }
            }

            DELEGATE_MAP = delegateMap;                
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckEventRouter"/> class.
        /// </summary>
        /// <param name="client">The Stream Deck client.</param>
        public StreamDeckEventRouter(StreamDeckClient client)
        {
            this.Client = client;
        }

        /// <summary>
        /// Gets the delegate map; this is used to determine which delegate should be invokved when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private static IDictionary<string, StreamDeckEventInfo> DELEGATE_MAP { get; }

        /// <summary>
        /// Gets the Stream Deck client.
        /// </summary>
        private StreamDeckClient Client { get; }

        /// <summary>
        /// Gets the registered actions.
        /// </summary>
        private IDictionary<string, Type> RegisteredActions { get; } = new Dictionary<string, Type>();

        /// <summary>
        /// Gets the actions that have been initialized, and can be invoked when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private ConcurrentDictionary<string, StreamDeckAction> Actions { get; } = new ConcurrentDictionary<string, StreamDeckAction>();

        /// <summary>
        /// Registers a new <see cref="StreamDeckAction"/> for the specified action UUID.
        /// </summary>
        /// <typeparam name="T">The type of Stream Deck action.</typeparam>
        /// <param name="actionUUID">The action UUID.</param>
        public void Register<T>(string actionUUID)
            => this.RegisteredActions.Add(actionUUID, typeof(T));

        /// <summary>
        /// Routes the message specified within the event arguments, invoking any associated delegates where possible.
        /// </summary>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs"/> instance containing the event data.</param>
        public void Route(WebSocketMessageEventArgs e)
        {
            // attempt to get the event delegate
            var eventArgs = JsonConvert.DeserializeObject<StreamDeckEventArgs>(e.Message);
            if (!DELEGATE_MAP.TryGetValue(eventArgs.Event, out var streamDeckEventInfo))
            {
                return;
            }

            // when the delegate is not an action, invoke the default handler
            var args = JsonConvert.DeserializeObject(e.Message, streamDeckEventInfo.MethodInfo.GetParameters()[0].ParameterType);
            if (!(args is IActionEventInfo actionInfo))
            {
                streamDeckEventInfo.MethodInfo.Invoke(this.Client, new[] { args });
                return;
            }

            var action = this.GetActionOrDefault(actionInfo);
            if (action != null)
            {
                streamDeckEventInfo.MethodInfo?.Invoke(action, new[] { args });
            }
        }

        /// <summary>
        /// Gets the <see cref="StreamDeckAction"/> for the specified <paramref name="actionInfo"/>, otherwise the default is return.
        /// </summary>
        /// <param name="actionInfo">The action information.</param>
        /// <returns>The action instance, a new instance of an action, or the default.</returns>
        private ActionEventHandler GetActionOrDefault(IActionEventInfo actionInfo)
        {
            // when there is no registered action for the action UUID, return the default handler
            if (!this.RegisteredActions.TryGetValue(actionInfo.Action, out var type))
            {
                return this.Client;
            }

            // otherwise attempt to get the instance of the action or initiate a new one
            return this.Actions.GetOrAdd(actionInfo.Context, context =>
            {
                var action = (StreamDeckAction)Activator.CreateInstance(type);
                action.Initialize(actionInfo, this.Client);

                return action;
            });
        }
    }
}
