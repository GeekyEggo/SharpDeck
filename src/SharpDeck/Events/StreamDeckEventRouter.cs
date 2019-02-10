namespace SharpDeck.Events
{
    using Extensions;
    using Net;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A factory that provides information about supported events received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventRouter : IDisposable
    {
        /// <summary>
        /// Gets the actions factory, used to initialize new instances of actions.
        /// </summary>
        private IDictionary<string, Func<StreamDeckAction>> ActionFactory { get; } = new Dictionary<string, Func<StreamDeckAction>>();

        /// <summary>
        /// Gets the actions that have been initialized, and can be invoked when a specific event is received from an Elgato Stream Deck.
        /// </summary>
        private ConcurrentDictionary<string, StreamDeckAction> Actions { get; } = new ConcurrentDictionary<string, StreamDeckAction>();

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
        public void Register<T>(string actionUUID, Func<T> valueFactory)
            where T : StreamDeckAction
            => this.ActionFactory.Add(actionUUID, valueFactory);

        /// <summary>
        /// Routes the message specified within the event arguments, invoking any associated delegates where possible.
        /// </summary>
        /// <param name="client">The Stream Deck client.</param>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs" /> instance containing the event data.</param>
        public Task RouteAsync(StreamDeckClient client, WebSocketMessageEventArgs e)
        {
            // determine if there is an event specified
            var args = JObject.Parse(e.Message);
            if (!args.TryGetString(nameof(StreamDeckEventArgs.Event), out var @event))
            {
                return Task.CompletedTask;
            }

            // when the event is not an action, allow the default Stream Deck client to handle the event
            if (!args.TryGetString(nameof(ActionEventArgs<object>.Action), out var actionUUID)
                || !args.TryGetString(nameof(ActionEventArgs<object>.Context), out var context))
            {
                return client.RaiseEventAsync(@event, args);
            }

            // otherwise get the action, and try to handle the event; device is not specified when "sendToPlugin" is called
            args.TryGetString(nameof(ActionEventArgs<object>.Device), out var device);
            return this.GetActionOrClient(actionUUID, context, device, client)
                .RaiseEventAsync(@event, args);
        }

        /// <summary>
        /// Gets the <see cref="StreamDeckAction"/> for the specified parameters; otherwise the default is returned.
        /// </summary>
        /// <param name="actionUUID">The actions unique identifier.</param>
        /// <param name="context">The opaque value identifying the instance of the action.</param>
        /// <param name="device">The opaque value identifying the device.</param>
        /// <param name="client">The Stream Deck client.</param>
        /// <returns>The action instance, a new instance of an action, or the default.</returns>
        private StreamDeckActionReceiver GetActionOrClient(string actionUUID, string context, string device, StreamDeckClient client)
        {
            // when there is no registered action for the action UUID, return the default handler
            if (!this.ActionFactory.TryGetValue(actionUUID, out var valueFactory))
            {
                return client;
            }

            // otherwise attempt to get the instance of the action or initiate a new one
            return this.Actions.GetOrAdd(context, _ =>
            {
                var action = valueFactory();
                action.Initialize(actionUUID, context, device, client);

                return action;
            });
        }
    }
}
