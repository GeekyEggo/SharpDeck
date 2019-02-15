namespace SharpDeck.Tests.Mocks
{
    using SharpDeck.Events;
    using SharpDeck.Events.PropertyInspectors;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a mock implementation of <see cref="StreamDeckAction"/>.
    /// </summary>
    public class FooStreamDeckAction : StreamDeckAction
    {
        /// <summary>
        /// The <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> and <see cref="PropertyInspectorMethodAttribute.SendToPropertyInspectorEvent"/> for <see cref="FooStreamDeckAction.PropertyInspector_SyncVoid(FooPropertyInspectorPayload)"/>.
        /// </summary>
        public const string SYNC_VOID_EVENT = "sync_void";

        /// <summary>
        /// The <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> for <see cref="FooStreamDeckAction.PropertyInspector_SyncResult(FooPropertyInspectorPayload)"/>.
        /// </summary>
        public const string SYNC_RESULT_EVENT_TO_PLUGIN = "sync_result_to_plugin";

        /// <summary>
        /// The <see cref="PropertyInspectorMethodAttribute.SendToPropertyInspectorEvent"/> for <see cref="FooStreamDeckAction.PropertyInspector_SyncResult(FooPropertyInspectorPayload)"/>.
        /// </summary>
        public const string SYNC_RESULT_EVENT_TO_PROPERTY_INSPECTOR = "sync_result_to_property_inspector";

        /// <summary>
        /// The <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> and <see cref="PropertyInspectorMethodAttribute.SendToPropertyInspectorEvent"/> for <see cref="FooStreamDeckAction.PropertyInspector_AsyncVoid(FooPropertyInspectorPayload)"/>.
        /// </summary>
        public const string ASYNC_VOID_EVENT = "async_void";

        /// <summary>
        /// The <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> and <see cref="PropertyInspectorMethodAttribute.SendToPropertyInspectorEvent"/> for <see cref="FooStreamDeckAction.PropertyInspector_AsyncResult(FooPropertyInspectorPayload)"/>.
        /// </summary>
        public const string ASYNC_RESULT_EVENT = "async_result";

        /// <summary>
        /// Gets or sets the method call count for each method.
        /// </summary>
        public Dictionary<string, int> MethodCallCount { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Gets the overall method call count.
        /// </summary>
        public int OverallMethodCallCount
        {
            get { return this.MethodCallCount.Values.Sum(); }
        } 

        /// <summary>
        /// A synchronous method without a result; the <see cref="MethodCallCount"/> is increment.
        /// </summary>
        /// <param name="args">The <see cref="FooPropertyInspectorPayload"/> instance containing the event data.</param>
        [PropertyInspectorMethod(SYNC_VOID_EVENT)]
        public void PropertyInspector_SyncVoid(FooPropertyInspectorPayload args)
            => this.IncrementMethodCallCount(args);

        /// <summary>
        /// A synchronous test method with a result; the <see cref="MethodCallCount"/> is increment.
        /// </summary>
        /// <param name="args">The <see cref="FooPropertyInspectorPayload"/> instance containing the event data.</param>
        /// <returns>The name of the method.</returns>
        [PropertyInspectorMethod(SYNC_RESULT_EVENT_TO_PLUGIN, SYNC_RESULT_EVENT_TO_PROPERTY_INSPECTOR)]
        public FooPropertyInspectorPayload PropertyInspector_SyncResult(FooPropertyInspectorPayload args)
        {
            this.IncrementMethodCallCount(args);
            return new FooPropertyInspectorPayload
            {
                Source = nameof(PropertyInspector_SyncResult)
            };
        }

        /// <summary>
        /// An asynchronous method without a result; the <see cref="MethodCallCount"/> is increment.
        /// </summary>
        /// <param name="args">The <see cref="FooPropertyInspectorPayload"/> instance containing the event data.</param>
        /// <returns>A completed task.</returns>
        [PropertyInspectorMethod(ASYNC_VOID_EVENT)]
        public Task PropertyInspector_AsyncVoid(FooPropertyInspectorPayload args)
        {
            this.IncrementMethodCallCount(args);
            return Task.CompletedTask;
        }

        /// <summary>
        /// An asynchronous method with a result; the <see cref="MethodCallCount"/> is increment.
        /// </summary>
        /// <param name="args">The <see cref="FooPropertyInspectorPayload"/> instance containing the event data.</param>
        /// <returns>The name of the method.</returns>
        [PropertyInspectorMethod(ASYNC_RESULT_EVENT)]
        public async Task<FooPropertyInspectorPayload> PropertyInspector_AsyncResult(FooPropertyInspectorPayload args)
        {
            this.IncrementMethodCallCount(args);
            await Task.Delay(1);

            return new FooPropertyInspectorPayload
            {
                Source = nameof(PropertyInspector_AsyncResult)
            };
        }

        /// <summary>
        /// Increments <see cref="MethodCallCount"/> for the specified <paramref name="methodName"/>.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        private void IncrementMethodCallCount(FooPropertyInspectorPayload args, [CallerMemberName] string methodName = "")
        {
            // ensure there is a method
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentException("Argument cannot be null or whitespace", nameof(methodName));
            }

            // add the entry if this is the first call
            if (!this.MethodCallCount.ContainsKey(methodName))
            {
                this.MethodCallCount.Add(methodName, 0);
            }

            this.MethodCallCount[methodName]++;
        }
    }
}
