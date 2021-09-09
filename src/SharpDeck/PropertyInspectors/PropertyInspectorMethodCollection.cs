namespace SharpDeck.PropertyInspectors
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
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
        /// <param name="logger">The optional logger.</param>
        public PropertyInspectorMethodCollection(Type type, ILogger logger = null)
        {
            // Add all methods that are decorated with the attribute.
            this.Logger = logger;
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
        /// Gets the logger.
        /// </summary>
        private ILogger Logger { get; }

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
            try
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

                // When the method has a result, send it to the property inspector.
                if (piMethodInfo.HasResult)
                {
                    args.Payload.TryGetString(nameof(PropertyInspectorPayload.RequestId), out var requestId);
                    var result = this.TryGetResultWithContext(task.Result, piMethodInfo, requestId);
                    await action.SendToPropertyInspectorAsync(result);
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, $"Failed to invoke property inspector method for \"{action.ActionUUID}\" ({action.Context}).");
                await action.ShowAlertAsync();
            }
        }

        /// <summary>
        /// Determines whether the specified result, as an object, inherits from <see cref="PropertyInspectorPayload"/> so that the event name can be set.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="methodInfo">The property inspector method information.</param>
        /// <param name="requestId">The request identifier from the original request.</param>
        /// <returns>The result to be sent to the property inspector.</returns>
        private object TryGetResultWithContext(object result, PropertyInspectorMethodInfo methodInfo, string requestId)
        {
            // Attempt to update the event name when the result is a payload.
            if (result is PropertyInspectorPayload payload)
            {
                payload.Event = methodInfo.SendToPropertyInspectorEvent;
                payload.RequestId = requestId;
                return payload;
            }

            return result;
        }
    }
}
