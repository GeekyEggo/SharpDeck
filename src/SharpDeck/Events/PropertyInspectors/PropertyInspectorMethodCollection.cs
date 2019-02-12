namespace SharpDeck.Events.PropertyInspectors
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection that maintains information about <see cref="PropertyInspectorMethodInfo"/> associated with an action.
    /// </summary>
    internal class PropertyInspectorMethodCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorMethodCollection"/> class.
        /// </summary>
        /// <param name="type">The <see cref="StreamDeckAction"/> type.</param>
        public PropertyInspectorMethodCollection(Type type)
        {
            // add all methods that are decorated with the attribute
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
            // attempt to get the method information
            var @event = args.Payload?.ToObject<PropertyInspectorPayload>()?.Event;
            if (string.IsNullOrWhiteSpace(@event) || !this.Methods.TryGetValue(@event, out var piMethodInfo))
            {
                return;
            }

            // invoke and await the method
            var task = piMethodInfo.InvokeAsync(action, args);
            await task;

            // when the method has a result, send it to the property inspector
            if (piMethodInfo.HasResult)
            {
                var result = this.TryGetResultWithEvent(task.Result, piMethodInfo);
                await action.SendToPropertyInspectorAsync(result);
            }
        }

        /// <summary>
        /// Determines whether the specified result, as an object, inherits from <see cref="PropertyInspectorPayload"/> so that the event name can be set.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="methodInfo">The property inspector method information.</param>
        /// <returns>The result to be sent to the property inspector.</returns>
        private object TryGetResultWithEvent(object result, PropertyInspectorMethodInfo methodInfo)
        {
            // attempt to update the event name when the result is a payload
            if (result is PropertyInspectorPayload payload)
            {
                payload.Event = methodInfo.SendToPropertyInspectorEvent;
                return payload;
            }

            return result;
        }
    }
}
