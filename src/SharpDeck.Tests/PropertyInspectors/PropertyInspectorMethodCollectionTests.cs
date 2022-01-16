namespace SharpDeck.Tests.PropertyInspectors
{
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;
    using SharpDeck.Interactivity;
    using SharpDeck.PropertyInspectors;
    using SharpDeck.PropertyInspectors.Payloads;
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
            => this.Collection = new PropertyInspectorMethodCollection(typeof(FooStreamDeckAction));

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
        [TestCase(FooStreamDeckAction.SYNC_RESULT_EVENT, ExpectedResult = true)]
        [TestCase(FooStreamDeckAction.SYNC_VOID_EVENT, ExpectedResult = true)]
        public async Task<bool> TestInvokeAsync_FindsEvent(string @event)
        {
            // Given.
            var action = new FooStreamDeckAction();
            action.Initialize(CONTEXT, new Mock<IStreamDeckConnection>().Object, new Mock<IDynamicProfileFactory>().Object);
            var args = GetArgs(@event);

            // When, then.
            await this.Collection.InvokeAsync(action, args);
            return action.OverallMethodCallCount == 1;
        }

        /// <summary>
        /// Asserts that if a method associated with a <see cref="PropertyInspectorMethodAttribute"/> is invoked, a response is send to the stream deck.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <returns><c>true</c> when <see cref="IStreamDeckConnection.SendToPropertyInspectorAsync(string, string, object, CancellationToken)"/> was invoked; otherwise <c>false</c>.</returns>
        [TestCase(null, ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [TestCase("fooEvent", ExpectedResult = false)]
        [TestCase(FooStreamDeckAction.ASYNC_RESULT_EVENT, ExpectedResult = true)]
        [TestCase(FooStreamDeckAction.ASYNC_VOID_EVENT, ExpectedResult = true)]
        [TestCase(FooStreamDeckAction.SYNC_RESULT_EVENT, ExpectedResult = true)]
        [TestCase(FooStreamDeckAction.SYNC_VOID_EVENT, ExpectedResult = true)]
        public async Task<bool> TestInvokeAsync_SendToPropertyInspector(string eventName)
        {
            int callCount = 0;

            // Given.
            var connection = new Mock<IStreamDeckConnection>();
            connection.Setup(s => s.SendToPropertyInspectorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Callback<string, string, object, CancellationToken>((contextUUID, actionUUID, payload, CancellationToken) =>
                {
                    if (actionUUID == CONTEXT.Action
                        && contextUUID == CONTEXT.Context
                        && ((PropertyInspectorResponsePayload)payload).Event == eventName)
                    {
                        callCount++;
                    }
                })
                .Returns(Task.CompletedTask);

            var action = new FooStreamDeckAction();
            var args = GetArgs(eventName);
            action.Initialize(CONTEXT, connection.Object, new Mock<IDynamicProfileFactory>().Object);

            // When.
            await this.Collection.InvokeAsync(action, args);

            // Then.
            return callCount > 0;
        }

        /// <summary>
        /// Gets the mock <see cref="ActionEventArgs{TPayload}"/> arguments that can be used to invoke a method.
        /// </summary>
        /// <param name="event">The `sendToPlugin` event name.</param>
        /// <returns>The arguments.</returns>
        private static ActionEventArgs<JObject> GetArgs(string @event)
        {
            return new ActionEventArgs<JObject>
            {
                Payload = JObject.FromObject(new PropertyInspectorResponsePayload
                {
                    Event = @event
                })
            };
        }
    }
}
