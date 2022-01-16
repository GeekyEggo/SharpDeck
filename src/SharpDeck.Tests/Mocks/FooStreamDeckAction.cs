namespace SharpDeck.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using SharpDeck.PropertyInspectors;

    /// <summary>
    /// Provides a mock implementation of <see cref="StreamDeckAction"/>.
    /// </summary>
    public class FooStreamDeckAction : StreamDeckAction
    {
        /// <summary>
        /// The <see cref="PropertyInspectorMethodAttribute.EventName"/> for <see cref="FooStreamDeckAction.PropertyInspector_SyncVoid(FooPropertyInspectorPayload)"/>.
        /// </summary>
        public const string SYNC_VOID_EVENT = "sync_void";

        /// <summary>
        /// The <see cref="PropertyInspectorMethodAttribute.EventName"/> for <see cref="FooStreamDeckAction.PropertyInspector_SyncResult(FooPropertyInspectorPayload)"/>.
        /// </summary>
        public const string SYNC_RESULT_EVENT = "sync_result";

        /// <summary>
        /// The <see cref="PropertyInspectorMethodAttribute.EventName"/> for <see cref="FooStreamDeckAction.PropertyInspector_AsyncVoid(FooPropertyInspectorPayload)"/>.
        /// </summary>
        public const string ASYNC_VOID_EVENT = "async_void";

        /// <summary>
        /// The <see cref="PropertyInspectorMethodAttribute.EventName"/> for <see cref="FooStreamDeckAction.PropertyInspector_AsyncResult(FooPropertyInspectorPayload)"/>.
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
        [PropertyInspectorMethod(SYNC_VOID_EVENT)]
        public void PropertyInspector_SyncVoid()
            => this.IncrementMethodCallCount();

        /// <summary>
        /// A synchronous test method with a result; the <see cref="MethodCallCount"/> is increment.
        /// </summary>
        /// <returns>The name of this method.</returns>
        [PropertyInspectorMethod(SYNC_RESULT_EVENT)]
        public string PropertyInspector_SyncResult()
        {
            this.IncrementMethodCallCount();
            return nameof(PropertyInspector_SyncResult);
        }

        /// <summary>
        /// An asynchronous method without a result; the <see cref="MethodCallCount"/> is increment.
        /// </summary>
        /// <returns>A completed task.</returns>
        [PropertyInspectorMethod(ASYNC_VOID_EVENT)]
        public Task PropertyInspector_AsyncVoid()
        {
            this.IncrementMethodCallCount();
            return Task.CompletedTask;
        }

        /// <summary>
        /// An asynchronous method with a result; the <see cref="MethodCallCount"/> is increment.
        /// </summary>
        /// <returns>The name of this method.</returns>
        [PropertyInspectorMethod(ASYNC_RESULT_EVENT)]
        public async Task<string> PropertyInspector_AsyncResult()
        {
            this.IncrementMethodCallCount();
            await Task.Yield();

            return nameof(PropertyInspector_AsyncResult);
        }

        /// <summary>
        /// Increments <see cref="MethodCallCount"/> for the specified <paramref name="methodName"/>.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        private void IncrementMethodCallCount([CallerMemberName] string methodName = "")
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
