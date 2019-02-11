﻿namespace SharpDeck.Events
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a factory that maintains information about <see cref="PropertyInspectorMethodInfo"/> associated with an action.
    /// </summary>
    internal class PropertyInspectorMethodFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorMethodFactory"/> class.
        /// </summary>
        /// <param name="type">The <see cref="StreamDeckAction"/> type.</param>
        public PropertyInspectorMethodFactory(Type type)
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
        public Task InvokeAsync(StreamDeckAction action, ActionEventArgs<JObject> args)
        {
            // attempt to get the method information
            var @event = args.Payload?.ToObject<SendToPluginPayload>()?.Event;
            if (string.IsNullOrWhiteSpace(@event) || !this.Methods.TryGetValue(@event, out var piMethodInfo))
            {
                return Task.CompletedTask;
            }

            return Task.Factory.StartNew(async () =>
            {
                // invoke the method
                var result = piMethodInfo.ParameterInfo == null
                    ? piMethodInfo.MethodInfo.Invoke(action, null)
                    : piMethodInfo.MethodInfo.Invoke(action, new[] { args.Payload.ToObject(piMethodInfo.ParameterInfo.ParameterType) });

                // when the method information has a return type, send the response to the property inspector
                if (piMethodInfo.MethodInfo.ReturnType != typeof(void))
                {
                    result = this.TryGetResultWithEvent(result, piMethodInfo);
                    await action.SendToPropertyInspectorAsync(result);
                }
            });
        }

        /// <summary>
        /// Determines whether the specified result, as an object, inherits from <see cref="SendToPluginPayload"/> so that the event name can be set.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="methodInfo">The property inspector method information.</param>
        /// <returns>The result to be sent to the property inspector.</returns>
        private object TryGetResultWithEvent(object result, PropertyInspectorMethodInfo methodInfo)
        {
            if (result is SendToPluginPayload payload)
            {
                payload.Event = methodInfo.SendToPropertyInspectorEvent;
                return payload;
            }

            return result;
        }
    }
}
