namespace StreamDeck.Extensions.Tests.PropertyInspectors
{
    using System.Linq;
    using System.Text.Json.Nodes;
    using Microsoft.Extensions.Hosting;
    using StreamDeck.Extensions.Serialization;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeck.Extensions.PropertyInspectors.HostExtensions"/>.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class HostExtensionsTests
    {
        /// <summary>
        /// Provides per-test setup.
        /// </summary>
        [SetUp]
        public void Setup()
            => StreamDeck.Extensions.PropertyInspectors.HostExtensions.RegisteredDataSources.Clear();

        /// <summary>
        /// Asserts <see cref="StreamDeck.Extensions.PropertyInspectors.HostExtensions.MapPropertyInspectorDataSource(IHost, string, Delegate)"/> for synchronous return values.
        /// </summary>
        [Test]
        public void MapPropertyInspectorDataSource_Sync()
        {
            var items = new DataSourceItem[]
            {
                new DataSourceItem("value_1", "Value One"),
                new DataSourceItem("value_2", "Value Two"),
                new DataSourceItem("value_3", "Value Three"),
            };

            Verify(items, items);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeck.Extensions.PropertyInspectors.HostExtensions.MapPropertyInspectorDataSource(IHost, string, Delegate)"/> for asynchronous return values.
        /// </summary>
        [Test]
        public void MapPropertyInspectorDataSource_Async()
        {
            var items = new DataSourceItem[]
            {
                new DataSourceItem("value_1", "Value One"),
                new DataSourceItem("value_2", "Value Two"),
                new DataSourceItem("value_3", "Value Three"),
            };

            Verify(Task.FromResult(items), items);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeck.Extensions.PropertyInspectors.HostExtensions.MapPropertyInspectorDataSource(IHost, string, Delegate)"/> ignores unregistered events.
        /// </summary>
        [Test]
        public void IgnoresUnregisteredEvents()
        {
            // Arrange.
            var invoked = false;
            var host = new MockHost();

            host.Object.MapPropertyInspectorDataSource("event_one", () =>
            {
                invoked = true;
                return Enumerable.Empty<DataSourceItem>();
            });

            // Act.
            var eArgs = CreateArgs("other_event");
            host.Connection.Raise(c => c.SendToPlugin += null, host.Connection.Object, eArgs);

            // Assert.
            host.Dispatcher.Verify(d => d.Invoke(It.IsAny<Func<Task>>(), It.IsAny<string>()), Times.Never);
            host.Connection.Verify(d => d.SendToPropertyInspectorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DataSourceResponse>(), StreamDeckJsonContext.Default.DataSourceResponse, It.IsAny<CancellationToken>()), Times.Never);
            Assert.That(invoked, Is.False);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeck.Extensions.PropertyInspectors.HostExtensions.MapPropertyInspectorDataSource(IHost, string, Delegate)"/> ignores events that don't match the expected payload structure.
        /// </summary>
        [Test]
        public void IgnoresOtherDataStructures()
        {
            // Arrange.
            var invoked = false;
            var host = new MockHost();

            host.Object.MapPropertyInspectorDataSource("event_one", () =>
            {
                invoked = true;
                return Enumerable.Empty<DataSourceItem>();
            });

            // Act.
            host.Connection.Raise(c => c.SendToPlugin += null, host.Connection.Object, new PartialActionEventArgs<JsonObject>("sendToPlugin", new JsonObject(), "com.tests.pi.action", "ABC123"));

            // Assert.
            host.Dispatcher.Verify(d => d.Invoke(It.IsAny<Func<Task>>(), It.IsAny<string>()), Times.Never);
            host.Connection.Verify(d => d.SendToPropertyInspectorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DataSourceResponse>(), StreamDeckJsonContext.Default.DataSourceResponse, It.IsAny<CancellationToken>()), Times.Never);
            Assert.That(invoked, Is.False);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeck.Extensions.PropertyInspectors.HostExtensions.MapPropertyInspectorDataSource(IHost, string, Delegate)"/> requires a <see cref="IEnumerable{DataSourceItem}"/> return type.
        /// </summary>
        [Test]
        public void RequireEnumerable()
        {
            // Arrange.
            var host = new MockHost();

            // Act, assert.
            Assert.Throws<NotSupportedException>(() => host.Object.MapPropertyInspectorDataSource("foo", () => { /* void */ }));

            Assert.Throws<NotSupportedException>(() => host.Object.MapPropertyInspectorDataSource("foo", () => "string"));
            Assert.Throws<NotSupportedException>(() => host.Object.MapPropertyInspectorDataSource("foo", () => Task.FromResult("string")));

            Assert.Throws<NotSupportedException>(() => host.Object.MapPropertyInspectorDataSource("foo", () => false));
            Assert.Throws<NotSupportedException>(() => host.Object.MapPropertyInspectorDataSource("foo", () => Task.FromResult(false)));

            Assert.Throws<NotSupportedException>(() => host.Object.MapPropertyInspectorDataSource("foo", () => new object()));
            Assert.Throws<NotSupportedException>(() => host.Object.MapPropertyInspectorDataSource("foo", () => Task.FromResult(new object())));
        }

        /// <summary>
        /// Asserts <see cref="StreamDeck.Extensions.PropertyInspectors.HostExtensions.MapPropertyInspectorDataSource(IHost, string, Delegate)"/> throws when an event has already been registerd.
        /// </summary>
        [Test]
        public void EnsuresEventsAreUnique()
        {
            // Arrange.
            const string EVENT_NAME = "event_one";

            var host = new MockHost();
            host.Object.MapPropertyInspectorDataSource(EVENT_NAME, () => Enumerable.Empty<DataSourceItem>());

            // Act, assert.
            Assert.Throws<DuplicateDataSourceException>(() => host.Object.MapPropertyInspectorDataSource(EVENT_NAME, () => Task.FromResult(Enumerable.Empty<DataSourceItem>())));
        }

        /// <summary>
        /// Verifies that when <see cref="StreamDeck.Extensions.PropertyInspectors.HostExtensions.MapPropertyInspectorDataSource(IHost, string, Delegate)"/> is configured,
        /// the handler is correctly invoked, and the data source is send back to the property inspector.
        /// </summary>
        /// <typeparam name="T">The type of the response from the delegate.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="expectedItems">The expected items.</param>
        private static void Verify<T>(T items, IEnumerable<DataSourceItem> expectedItems)
        {
            // Arrange.
            var eArgs = CreateArgs("getItems");

            var host = new MockHost();
            host.Object.MapPropertyInspectorDataSource("getItems", Handler);

            // Act
            host.Connection.Raise(c => c.SendToPlugin += null, host.Connection.Object, eArgs);

            // Assert
            host.Dispatcher.Verify(d => d.Invoke(It.IsAny<Func<Task>>(), eArgs.Context), Times.Once);
            host.Connection.Verify(
                c => c.SendToPropertyInspectorAsync(
                    eArgs.Context,
                    eArgs.Action,
                    It.Is<DataSourceResponse>(d => d.Event == "getItems" && d.Items.SequenceEqual(expectedItems)),
                    StreamDeckJsonContext.Default.DataSourceResponse,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            T Handler(IStreamDeckConnection connection, PartialActionEventArgs<JsonObject> args, IService service)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(connection, Is.EqualTo(host.Connection.Object));
                    Assert.That(args, Is.EqualTo(eArgs));
                    Assert.That(args.GetPayload<DataSourcePayload>()!.Event, Is.EqualTo("getItems"));
                    Assert.That(service, Is.EqualTo(host.Service));
                });

                return items;
            }
        }

        /// <summary>
        /// Creates a set of <see cref="PartialActionEventArgs{JsonObject}"/> that can be used to invoke <see cref="IStreamDeckConnection.SendToPlugin"/>.
        /// </summary>
        /// <param name="event">The name of the event represented within the payloads <see cref="DataSourcePayload"/>.</param>
        /// <param name="context">The context of the action.</param>
        /// <returns>The event arguments.</returns>
        private static PartialActionEventArgs<JsonObject> CreateArgs(string @event, string context = "ABC123")
        {
            var payload = new JsonObject(new[] { new KeyValuePair<string, JsonNode?>("event", @event) });
            return new PartialActionEventArgs<JsonObject>("sendToPlugin", payload, "com.tests.pi.action", context);
        }
    }
}
