namespace SharpDeck.Tests.Events.PropertyInspectors
{
    using System.Threading.Tasks;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using SharpDeck.Events.Received;
    using SharpDeck.PropertyInspectors;
    using SharpDeck.Tests.Mocks;

    /// <summary>
    /// Provides assertions for expected behaviour within <see cref="PropertyInspectorMethodCollection"/>.
    /// </summary>
    [TestFixture]
    public class PropertyInspectorMethodCollectionTests
    {
        /// <summary>
        /// Gets the mock context.
        /// </summary>
        private static ActionEventArgs<AppearancePayload> CONTEXT { get; } = new ActionEventArgs<AppearancePayload>
        {
            Action = "ACTION_MOCK_UUID",
            Context = "CONTEXT_MOCK_UUID",
            Device = "DEVICE_MOCK_UUID",
            Payload = new AppearancePayload()
        };

        /// <summary>
        /// Gets or sets the collection, used as the primary test case.
        /// </summary>
        private PropertyInspectorMethodCollection Collection { get; set; }

        /// <summary>
        /// Provides one time set-up for the test fixture.
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.Collection = new PropertyInspectorMethodCollection(typeof(FooStreamDeckAction));
        }

        /// <summary>
        /// Asserts that <see cref="PropertyInspectorMethodCollection.InvokeAsync(StreamDeckAction, ActionEventArgs{JObject})"/> correctly finds, and does not find, events base on their name.
        /// </summary>
        /// <param name="event">The event name.</param>
        /// <returns><c>true</c> when the method was invoked; otherwise <c>false</c>.</returns>
        [TestCase(null, ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [TestCase("fooEvent", ExpectedResult = false)]
        [TestCase(FooStreamDeckAction.ASYNC_RESULT_EVENT, ExpectedResult = true)]
        [TestCase(FooStreamDeckAction.ASYNC_VOID_EVENT, ExpectedResult = true)]
        [TestCase(FooStreamDeckAction.SYNC_RESULT_EVENT_TO_PLUGIN, ExpectedResult = true)]
        [TestCase(FooStreamDeckAction.SYNC_VOID_EVENT, ExpectedResult = true)]
        public async Task<bool> TestInvokeAsync_FindsEvent(string @event)
        {
            // given
            var action = new FooStreamDeckAction();
            action.Initialize(CONTEXT, new Mock<IStreamDeckConnection>().Object);
            var args = this.GetArgs(@event);

            // when, then
            await this.Collection.InvokeAsync(action, args);
            return action.OverallMethodCallCount == 1;
        }

        /// <summary>
        /// Asserts that if a method associated with a <see cref="PropertyInspectorMethodAttribute"/> returns a result, that <see cref="IStreamDeckSender.SendToPropertyInspectorAsync(string, string, object)"/> is invoked when calling <see cref="PropertyInspectorMethodCollection.InvokeAsync(StreamDeckAction, ActionEventArgs{JObject})"/>.
        /// </summary>
        /// <param name="sendToPluginEvent">The `sendToPlugin` event name.</param>
        /// <param name="sendToPropertyInspectorEvent">The `sendToPropertyInspector` event name.</param>
        /// <returns><c>true</c> when <see cref="IStreamDeckSender.SendToPropertyInspectorAsync(string, string, object)"/> was invoked; otherwise <c>false</c>.</returns>
        [TestCase(null, null, ExpectedResult = false)]
        [TestCase("", null, ExpectedResult = false)]
        [TestCase("fooEvent", null, ExpectedResult = false)]
        [TestCase(FooStreamDeckAction.ASYNC_RESULT_EVENT, FooStreamDeckAction.ASYNC_RESULT_EVENT, ExpectedResult = true)]
        [TestCase(FooStreamDeckAction.ASYNC_VOID_EVENT, null, ExpectedResult = false)]
        [TestCase(FooStreamDeckAction.SYNC_RESULT_EVENT_TO_PLUGIN, FooStreamDeckAction.SYNC_RESULT_EVENT_TO_PROPERTY_INSPECTOR, ExpectedResult = true)]
        [TestCase(FooStreamDeckAction.SYNC_VOID_EVENT, null, ExpectedResult = false)]
        public async Task<bool> TestInvokeAsync_SendToPropertyInspector(string sendToPluginEvent, string sendToPropertyInspectorEvent)
        {
            int callCount = 0;

            // given
            var streamDeckSender = new Mock<IStreamDeckConnection>();
            streamDeckSender.Setup(s => s.SendToPropertyInspectorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, string, object>((contextUUID, actionUUID, payload) => {
                    if (actionUUID == CONTEXT.Action && contextUUID == CONTEXT.Context && ((FooPropertyInspectorPayload)payload).Event == sendToPropertyInspectorEvent)
                    {
                        callCount++;
                    }
                }).Returns(() => Task.CompletedTask);

            var action = new FooStreamDeckAction();
            var args = this.GetArgs(sendToPluginEvent);
            action.Initialize(CONTEXT, streamDeckSender.Object);

            // when
            await this.Collection.InvokeAsync(action, args);

            // then
            return callCount > 0;
        }

        /// <summary>
        /// Gets the mock <see cref="ActionEventArgs{TPayload}"/> arguments that can be used to invoke a method.
        /// </summary>
        /// <param name="event">The `sendToPlugin` event name.</param>
        /// <returns>The arguments.</returns>
        private ActionEventArgs<JObject> GetArgs(string @event)
        {
            return new ActionEventArgs<JObject>
            {
                Payload = JObject.FromObject(new PropertyInspectorPayload
                {
                    Event = @event
                })
            };
        }
    }
}
