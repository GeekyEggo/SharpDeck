namespace StreamDeck.Extensions.Tests.PropertyInspectors
{
    using StreamDeck.Net;
    using StreamDeck.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeck.Extensions.PropertyInspectors.StreamDeckConnectionExtensions"/>.
    /// </summary>
    [TestFixture]
    public class StreamDeckConnectionExtensionsTests
    {
        /// <summary>
        /// Asserts <see cref="StreamDeck.Extensions.PropertyInspectors.StreamDeckConnectionExtensions.SendDataSourceToPropertyInspectorAsync(IStreamDeckConnection, string, string, string, IEnumerable{DataSourceItem}, CancellationToken)"/>.
        /// </summary>
        /// <returns>The task of running the test.</returns>
        [Test]
        public async Task SendDataSourceToPropertyInspectorAsync()
        {
            // Arrange.
            var webSocket = new Mock<IWebSocketConnection>();
            var connection = new StreamDeckConnection(RegistrationParametersHelper.CreateRegistrationParameters(), webSocket.Object);

            var cancellationToken = new CancellationToken();
            var items = new[]
            {
                new DataSourceItem("val_1", "Value One", disabled: true),
                new DataSourceItem("Group 1", new[] { new DataSourceItem("nested_1", "Nested One") })
            };

            // Act.
            await connection.SendDataSourceToPropertyInspectorAsync(
                context: "ABC123",
                action: "com.tests.pi.one",
                @event: "getItems",
                items,
                cancellationToken);

            // Assert.
            webSocket.Verify(ws => ws.SendAsync("""
                {"action":"com.tests.pi.one","context":"ABC123","payload":{"event":"getItems","items":[{"disabled":true,"label":"Value One","value":"val_1"},{"children":[{"label":"Nested One","value":"nested_1"}],"label":"Group 1"}]},"event":"sendToPropertyInspector"}
                """, cancellationToken), Times.Once);
        }
    }
}
