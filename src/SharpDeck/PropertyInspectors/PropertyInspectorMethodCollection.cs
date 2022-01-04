namespace SharpDeck.PropertyInspectors
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Events.Received;
    using SharpDeck.Extensions;

    /// <summary>
    /// Provides a collection that maintains information about <see cref="PropertyInspectorMethodInfo"/> associated with an action.
    /// </summary>
    public class PropertyInspectorMethodCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorMethodCollection"/> class.
        /// </summary>
        /// <param name="type">The <see cref="StreamDeckAction"/> type.</param>
        public PropertyInspectorMethodCollection(Type type)
        {
            // Add all methods that are decorated with the attribute.
            foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attr = methodInfo.GetCustomAttribute<PropertyInspectorMethodAttribute>();
                if (attr != null)
                {
                    var piMethodInfo = new PropertyInspectorMethodInfo(methodInfo, attr);
                    this.Methods.Add(piMethodInfo.SendToPluginEvent, piMethodInfo);
                }
            }
        }

        /// <summary>
        /// Gets the property inspector methods.
        /// </summary>
        private Dictionary<string, PropertyInspectorMethodInfo> Methods { get; } = new Dictionary<string, PropertyInspectorMethodInfo>();

        /// <summary>
        /// Invokes the method associated with <see cref="StreamDeckEventArgs{TPayload}.Payload"/>, specifically the property `event`, for the given action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="args">The <see cref="ActionEventArgs{JObject}" /> instance containing the event data.</param>
        public async Task InvokeAsync(StreamDeckAction action, ActionEventArgs<JObject> args)
        {
            // Attempt to get the method information.
            args.Payload.TryGetString(nameof(PropertyInspectorPayload.Event), out var @event);
            if (string.IsNullOrWhiteSpace(@event) || !this.Methods.TryGetValue(@event, out var piMethodInfo))
            {
                return;
            }

            // Invoke and await the method.
            var task = piMethodInfo.InvokeAsync(action, args);
            await task;

            // When the method was sent with a request identifier, we can attempt a response.
            if (args.Payload.TryGetString(nameof(PropertyInspectorPayload.RequestId), out var requestId))
            {
                var payload = new PropertyInspectorPayload
                {
                    Event = piMethodInfo.SendToPropertyInspectorEvent,
                    RequestId = requestId
                };

                if (piMethodInfo.HasResult)
                {
                    payload.Data = task.Result;
                }

                await action.SendToPropertyInspectorAsync(payload);
            }
        }
    }
}
