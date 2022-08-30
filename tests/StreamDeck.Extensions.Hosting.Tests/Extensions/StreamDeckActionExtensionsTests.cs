namespace StreamDeck.Tests.Extensions
{
    using Moq;
    using StreamDeck.Extensions;
    using StreamDeck.Tests.Helpers;
    using StreamDeck.Tests.Serialization;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeckActionExtensions"/>.
    /// </summary>
    [TestFixture]
    public class StreamDeckActionExtensionsTests
    {
        /// <summary>
        /// Gets the test cases used to assert a <see cref="StreamDeckAction"/> method throws <see cref="InvalidOperationException"/> when it cannot communicate with the Stream Deck.
        /// </summary>
        public static IEnumerable<TestCaseData> NotInitializedTestCases
        {
            get
            {
                const string actionUUID = "com.tests.plugin.action";
                const string context = "ABC123";
                var connection = new Mock<IStreamDeckConnection>().Object;

                static TestCaseData TestCase(string name, string? actionUUID = null, string? context = null, IStreamDeckConnection? connection = null)
                {
                    return new TestCaseData(new StreamDeckAction
                    {
                        ActionUUID = actionUUID,
                        Context = context,
                        Connection = connection
                    }).SetName($"{{m}} ({name})");
                }

                yield return TestCase("ActionUUID, Context, and Connection null");
                yield return TestCase("Context and Connection null", actionUUID: actionUUID);
                yield return TestCase("Connection null", actionUUID: actionUUID, context: context);
                yield return TestCase("Context null", actionUUID: actionUUID, connection: connection);
                yield return TestCase("ActionUUID and Connection null", context: context);
                yield return TestCase("ActionUUID null", context: context, connection: connection);
                yield return TestCase("ActionUUID and Context null", connection: connection);
            }
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.GetSettingsAsync(StreamDeckAction, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task GetSettingsAsync()
        {
            // Arrange.
            var (action, connection) = CreateTestCase();
            var cancellationToken = new CancellationToken();

            // Act.
            await action.GetSettingsAsync(cancellationToken);

            // Assert.
            connection.Verify(c => c.GetSettingsAsync(action.Context!, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.GetSettingsAsync(StreamDeckAction, CancellationToken)"/> throws <see cref="InvalidOperationException"/> when the action cannot communicate with the Stream Deck.
        /// </summary>
        /// <param name="action">The action that is not connected to the Stream Deck.</param>
        [Test, TestCaseSource(nameof(NotInitializedTestCases))]
        public void GetSettingsAsync_NotInitialized(StreamDeckAction action)
            => Assert.ThrowsAsync<InvalidOperationException>(() => action.GetSettingsAsync(default));

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.SendToPropertyInspectorAsync{TPayload}(StreamDeckAction, TPayload, System.Text.Json.Serialization.Metadata.JsonTypeInfo{TPayload}?, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task SendToPropertyInspectorAsync()
        {
            // Arrange.
            var (action, connection) = CreateTestCase();
            var cancellationToken = new CancellationToken();
            var payload = new FooSettings { Name = "Bob Smith" };

            // Act.
            await action.SendToPropertyInspectorAsync(payload, TestJsonContext.Default.FooSettings, cancellationToken);

            // Assert.
            connection.Verify(c => c.SendToPropertyInspectorAsync(action.Context!, action.ActionUUID!, payload, TestJsonContext.Default.FooSettings, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.SendToPropertyInspectorAsync{TPayload}(StreamDeckAction, TPayload, System.Text.Json.Serialization.Metadata.JsonTypeInfo{TPayload}?, CancellationToken)"/> throws <see cref="InvalidOperationException"/> when the action cannot communicate with the Stream Deck.
        /// </summary>
        /// <param name="action">The action that is not connected to the Stream Deck.</param>
        [Test, TestCaseSource(nameof(NotInitializedTestCases))]
        public void SendToPropertyInspectorAsync_NotInitialized(StreamDeckAction action)
            => Assert.ThrowsAsync<InvalidOperationException>(() => action.SendToPropertyInspectorAsync<object>(null!, null, default));

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.SetImageAsync(StreamDeckAction, string, Target, int?, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task SetImageAsync()
        {
            // Arrange.
            var (action, connection) = CreateTestCase();
            var cancellationToken = new CancellationToken();
            const string image = "foo_bar";
            const Target target = Target.Hardware;
            const int state = 1;

            // Act.
            await action.SetImageAsync(image, target, state, cancellationToken);

            // Assert.
            connection.Verify(c => c.SetImageAsync(action.Context!, image, target, state, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.SetImageAsync(StreamDeckAction, string, Target, int?, CancellationToken)"/> throws <see cref="InvalidOperationException"/> when the action cannot communicate with the Stream Deck.
        /// </summary>
        /// <param name="action">The action that is not connected to the Stream Deck.</param>
        [Test, TestCaseSource(nameof(NotInitializedTestCases))]
        public void SetImageAsync_NotInitialized(StreamDeckAction action)
            => Assert.ThrowsAsync<InvalidOperationException>(() => action.SetImageAsync("foo_bar", Target.Software, 1, default));

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.SetSettingsAsync{TSettings}(StreamDeckAction, TSettings, System.Text.Json.Serialization.Metadata.JsonTypeInfo{TSettings}?, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task SetSettingsAsync()
        {
            // Arrange.
            var (action, connection) = CreateTestCase();
            var cancellationToken = new CancellationToken();
            var payload = new FooSettings { Name = "Bob Smith" };

            // Act.
            await action.SetSettingsAsync(payload, TestJsonContext.Default.FooSettings, cancellationToken);

            // Assert.
            connection.Verify(c => c.SetSettingsAsync(action.Context!, payload, TestJsonContext.Default.FooSettings, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.SetSettingsAsync{TSettings}(StreamDeckAction, TSettings, System.Text.Json.Serialization.Metadata.JsonTypeInfo{TSettings}?, CancellationToken)"/> throws <see cref="InvalidOperationException"/> when the action cannot communicate with the Stream Deck.
        /// </summary>
        /// <param name="action">The action that is not connected to the Stream Deck.</param>
        [Test, TestCaseSource(nameof(NotInitializedTestCases))]
        public void SetSettingsAsync_NotInitialized(StreamDeckAction action)
            => Assert.ThrowsAsync<InvalidOperationException>(() => action.SetSettingsAsync<object>(null!, null, default));

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.SetStateAsync(StreamDeckAction, int, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task SetStateAsync()
        {
            // Arrange.
            var (action, connection) = CreateTestCase();
            var cancellationToken = new CancellationToken();
            const int state = 1;

            // Act.
            await action.SetStateAsync(state, cancellationToken);

            // Assert.
            connection.Verify(c => c.SetStateAsync(action.Context!, state, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.SetStateAsync(StreamDeckAction, int, CancellationToken)"/> throws <see cref="InvalidOperationException"/> when the action cannot communicate with the Stream Deck.
        /// </summary>
        /// <param name="action">The action that is not connected to the Stream Deck.</param>
        [Test, TestCaseSource(nameof(NotInitializedTestCases))]
        public void SetStateAsync_NotInitialized(StreamDeckAction action)
            => Assert.ThrowsAsync<InvalidOperationException>(() => action.SetStateAsync(1, default));

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.ShowOkAsync(StreamDeckAction, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task ShowOkAsync()
        {
            // Arrange.
            var (action, connection) = CreateTestCase();
            var cancellationToken = new CancellationToken();

            // Act.
            await action.ShowOkAsync(cancellationToken);

            // Assert.
            connection.Verify(c => c.ShowOkAsync(action.Context!, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.ShowOkAsync(StreamDeckAction, CancellationToken)"/> throws <see cref="InvalidOperationException"/> when the action cannot communicate with the Stream Deck.
        /// </summary>
        /// <param name="action">The action that is not connected to the Stream Deck.</param>
        [Test, TestCaseSource(nameof(NotInitializedTestCases))]
        public void ShowOkAsync_NotInitialized(StreamDeckAction action)
            => Assert.ThrowsAsync<InvalidOperationException>(() => action.ShowOkAsync(default));

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.ShowAlertAsync(StreamDeckAction, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task ShowAlertAsync()
        {
            // Arrange.
            var (action, connection) = CreateTestCase();
            var cancellationToken = new CancellationToken();

            // Act.
            await action.ShowAlertAsync(cancellationToken);

            // Assert.
            connection.Verify(c => c.ShowAlertAsync(action.Context!, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.ShowAlertAsync(StreamDeckAction, CancellationToken)"/> throws <see cref="InvalidOperationException"/> when the action cannot communicate with the Stream Deck.
        /// </summary>
        /// <param name="action">The action that is not connected to the Stream Deck.</param>
        [Test, TestCaseSource(nameof(NotInitializedTestCases))]
        public void ShowAlertAsync_NotInitialized(StreamDeckAction action)
            => Assert.ThrowsAsync<InvalidOperationException>(() => action.ShowAlertAsync(default));

        /// <summary>
        /// Creates a new test case.
        /// </summary>
        /// <returns>The <see cref="StreamDeckAction"/>, and the mock <see cref="IStreamDeckConnection"/> associated with it.</returns>
        private static (StreamDeckAction action, Mock<IStreamDeckConnection> connection) CreateTestCase()
        {
            var connection = new Mock<IStreamDeckConnection>();
            return (
                new StreamDeckAction
                {
                    ActionUUID = "com.tests.plugin.action",
                    Context = "ABC123",
                    Connection = connection.Object
                },
                connection);
        }
    }
}
