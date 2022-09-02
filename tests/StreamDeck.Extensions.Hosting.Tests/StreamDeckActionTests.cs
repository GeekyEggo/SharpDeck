namespace StreamDeck.Tests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using StreamDeck.Tests.Helpers;
    using StreamDeck.Tests.Serialization;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeckAction"/>.
    /// </summary>
    [TestFixture]
    public class StreamDeckActionTests
    {
        /// <summary>
        /// Asserts <see cref="StreamDeckAction(ActionInitializationContext)"/>.
        /// </summary>
        [Test]
        public void Construct()
        {
            // Arrange.
            var connection = new Mock<IStreamDeckConnection>();
            var logger = new Mock<ILogger>();
            var context = new ActionInitializationContext(connection.Object, EventArgsBuilder.GetActionEventArgs(), logger.Object);

            // Act.
            var action = new StreamDeckAction(context);

            // Assert.
            Assert.Multiple(() =>
            {
                Assert.That(action.ActionUUID, Is.EqualTo(context.ActionInfo.Action));
                Assert.That(action.Connection, Is.EqualTo(context.Connection));
                Assert.That(action.Context, Is.EqualTo(context.ActionInfo.Context));
                Assert.That(action.Logger, Is.EqualTo(context.Logger));
            });
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckAction.Dispose"/>.
        /// </summary>
        [Test]
        public void Dispose()
        {
            // Arrange.
            var (action, _) = CreateTestCase();

            // Act.
            action.Dispose();

            // Assert.
            Assert.Throws<ObjectDisposedException>(() => { var _ = action.Context; });
        }

        /// <summary>
        /// Asserts the virtual implementations within <see cref="StreamDeckAction" /> return <see cref="Task.CompletedTask" />.
        /// </summary>
        [Test]
        public void EventHandlerDefaults()
        {
            // Arrange.
            var (action, _) = CreateTestCase();

            // Act, assert.
            Assert.Multiple(() =>
            {
                Assert.That(action.OnDidReceiveSettings(null!), Is.SameAs(Task.CompletedTask));
                Assert.That(action.OnKeyDown(null!), Is.SameAs(Task.CompletedTask));
                Assert.That(action.OnKeyUp(null!), Is.SameAs(Task.CompletedTask));
                Assert.That(action.OnPropertyInspectorDidAppear(null!), Is.SameAs(Task.CompletedTask));
                Assert.That(action.OnPropertyInspectorDidDisappear(null!), Is.SameAs(Task.CompletedTask));
                Assert.That(action.OnSendToPlugin(null!), Is.SameAs(Task.CompletedTask));
                Assert.That(action.OnTitleParametersDidChange(null!), Is.SameAs(Task.CompletedTask));
                Assert.That(action.OnWillAppear(null!), Is.SameAs(Task.CompletedTask));
                Assert.That(action.OnWillDisappear(null!), Is.SameAs(Task.CompletedTask));
            });
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckAction.GetSettingsAsync(CancellationToken)"/>.
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
            connection.Verify(c => c.GetSettingsAsync(action.Context, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckAction.SendToPropertyInspectorAsync{TPayload}(TPayload, System.Text.Json.Serialization.Metadata.JsonTypeInfo{TPayload}?, CancellationToken)"/>.
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
            connection.Verify(c => c.SendToPropertyInspectorAsync(action.Context, action.ActionUUID, payload, TestJsonContext.Default.FooSettings, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckAction.SetImageAsync(string, Target, int?, CancellationToken)"/>.
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
            connection.Verify(c => c.SetImageAsync(action.Context, image, target, state, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckAction.SetSettingsAsync{TSettings}(TSettings, System.Text.Json.Serialization.Metadata.JsonTypeInfo{TSettings}?, CancellationToken)"/>.
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
            connection.Verify(c => c.SetSettingsAsync(action.Context, payload, TestJsonContext.Default.FooSettings, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckAction.SetStateAsync(int, CancellationToken)"/>.
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
            connection.Verify(c => c.SetStateAsync(action.Context, state, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckAction.SetTitleAsync(string, Target, int?, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task SetTitleAsync()
        {
            // Arrange.
            var (action, connection) = CreateTestCase();
            var cancellationToken = new CancellationToken();
            const string title = "Hello world";
            const Target target = Target.Hardware;
            const int state = 1;

            // Act.
            await action.SetTitleAsync(title, target, state, cancellationToken);

            // Assert.
            connection.Verify(c => c.SetTitleAsync(action.Context, title, target, state, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckAction.ShowOkAsync(CancellationToken)"/>.
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
            connection.Verify(c => c.ShowOkAsync(action.Context, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckAction.ShowAlertAsync(CancellationToken)"/>.
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
            connection.Verify(c => c.ShowAlertAsync(action.Context, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Creates a new test case.
        /// </summary>
        /// <returns>The <see cref="StreamDeckAction"/>, and the mock <see cref="IStreamDeckConnection"/> associated with it.</returns>
        private static (StreamDeckAction action, Mock<IStreamDeckConnection> connection) CreateTestCase()
        {
            var connection = new Mock<IStreamDeckConnection>();
            return (new StreamDeckAction(new ActionInitializationContext(connection.Object, EventArgsBuilder.GetActionEventArgs())), connection);
        }
    }
}
