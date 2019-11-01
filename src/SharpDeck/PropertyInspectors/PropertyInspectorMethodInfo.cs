namespace SharpDeck.PropertyInspectors
{
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Events;
    using SharpDeck.Events.Received;
    using SharpDeck.Extensions;

    /// <summary>
    /// Provides information about method that should be invoked when receiving a specific message from <see cref="StreamDeckActionEventPropagator.SendToPlugin"/>.
    /// </summary>
    public class PropertyInspectorMethodInfo
    {
        /// <summary>
        /// Invokes the method information asynchronously, returning the result within a task.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The result of invoking the method.</returns>
        public delegate Task<object> AsyncMethodInvoker(object sender, object[] parameters);

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspectorMethodInfo"/> class.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="attr">The attribute used to decorated the method.</param>
        public PropertyInspectorMethodInfo(MethodInfo methodInfo, PropertyInspectorMethodAttribute attr)
        {
            this.SendToPluginEvent = attr.SendToPluginEvent.OrDefault(methodInfo.Name);
            this.SendToPropertyInspectorEvent = attr.SendToPropertyInspectorEvent.OrDefault(this.SendToPluginEvent);

            this.MethodInfo = methodInfo;
            this.ParameterInfo = methodInfo.GetParameters().FirstOrDefault();
            this.HasResult = methodInfo.ReturnType != typeof(void) && methodInfo.ReturnType != typeof(Task);

            this.InternalInvokeAsync = this.GetInternalInvokeAsync();
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MethodInfo"/> has a result.
        /// </summary>
        public bool HasResult { get; private set; }

        /// <summary>
        /// Gets the `sendToPlugin` event name.
        /// </summary>
        public string SendToPluginEvent { get; }

        /// <summary>
        /// Gets the `sendToPropertyInspector` event name.
        /// </summary>
        public string SendToPropertyInspectorEvent { get; }

        /// <summary>
        /// Gets or sets the internal invoker used to asynchronously invoke <see cref="MethodInfo" />
        /// </summary>
        private AsyncMethodInvoker InternalInvokeAsync { get; set; }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        private MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets the information for the parameter that should be supplied to the method information.
        /// </summary>
        private ParameterInfo ParameterInfo { get; }

        /// <summary>
        /// Invokes the method asynchronously.
        /// </summary>
        /// <param name="action">The source of the event; the Stream Deck action.</param>
        /// <param name="args">The <see cref="ActionEventArgs{JObject}"/> instance containing the event data.</param>
        /// <returns>The result of invoking the <see cref="MethodInfo"/>.</returns>
        public Task<object> InvokeAsync(StreamDeckAction action, ActionEventArgs<JObject> args)
        {
            return this.ParameterInfo == null
                ? this.InternalInvokeAsync(action, null)
                : this.InternalInvokeAsync(action, new[] { args.Payload.ToObject(this.ParameterInfo.ParameterType) });
        }

        /// <summary>
        /// Gets the <see cref="AsyncMethodInvoker"/> that should be assigned to the <see cref="InternalInvokeAsync"/> based on the state of this instance.
        /// </summary>
        /// <returns>The <see cref="AsyncMethodInvoker"/> to invoke the <see cref="MethodInfo"/>, and get the result.</returns>
        private AsyncMethodInvoker GetInternalInvokeAsync()
        {
            // return the synchronous invoker
            if (!typeof(Task).IsAssignableFrom(this.MethodInfo.ReturnType))
            {
                return this.InvokeSync;
            }

            // otherwise return the asynchronous invoker, based on whether it contains a result
            var resultProp = this.MethodInfo.ReturnType.GetProperty(nameof(Task<object>.Result));
            return resultProp == null ? this.InvokeVoidAsync : this.GetResultAsyncInvoker(resultProp);
        }

        /// <summary>
        /// Gets the asynchronous invoker that will return the result of the resulted task.
        /// </summary>
        /// <param name="prop">The property detailing the <see cref="Task{TResult}.Result"/>.</param>
        /// <returns>The invoker to be used to asynchronously invoke <see cref="MethodInfo"/>.</returns>
        private AsyncMethodInvoker GetResultAsyncInvoker(PropertyInfo prop)
        {
            return async (sender, parameters) =>
            {
                var task = await this.InvokeVoidAsync(sender, parameters);
                var result = prop.GetValue(task);

                return result;
            };
        }

        /// <summary>
        /// Invokes the <see cref="MethodInfo"/> synchronously, returning the result as a task.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The result of the synchronous invokation.</returns>
        private Task<object> InvokeSync(object sender, object[] parameters)
            => Task.FromResult(this.MethodInfo.Invoke(sender, parameters));

        /// <summary>
        /// Invokes the <see cref="MethodInfo"/> asynchronously, where the result is void.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The result of the asynchronous invokation.</returns>
        private async Task<object> InvokeVoidAsync(object sender, object[] parameters)
        {
            var result = this.MethodInfo.Invoke(sender, parameters);
            await (Task)result;

            return result;
        }
    }
}
