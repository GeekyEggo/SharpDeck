namespace StreamDeck.Extensions.Tests.PropertyInspectors
{
    using StreamDeck.Net;
    using StreamDeck.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="StreamDeckActionExtensions"/>.
    /// </summary>
    [TestFixture]
    public class StreamDeckActionExtensionsTests
    {
        /// <summary>
        /// Asserts <see cref="StreamDeckActionExtensions.SendDataSourceToPropertyInspectorAsync(StreamDeckAction, string, IEnumerable{DataSourceItem}, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task SendDataSourceToPropertyInspectorAsync()
        {
            // Arrange.
            var webSocket = new Mock<IWebSocketConnection>();
            var connection = new StreamDeckConnection(RegistrationParametersHelper.CreateRegistrationParameters(), webSocket.Object);

            var action = new StreamDeckAction(new ActionInitializationContext(connection, EventArgsBuilder.CreateActionEventArgs()));

            var cancellationToken = new CancellationToken();
            var items = new[]
            {
                new DataSourceItem("val_1", "Value One", disabled: true),
                new DataSourceItem("Group 1", new[] { new DataSourceItem("nested_1", "Nested One") })
            };

            // Act.
            await action.SendDataSourceToPropertyInspectorAsync("getItems", items, cancellationToken);

            // Assert.
            webSocket.Verify(ws => ws.SendAsync($$$"""
                {"action":"{{{action.ActionUUID}}}","context":"{{{action.Context}}}","payload":{"items":[{"disabled":true,"label":"Value One","value":"val_1"},{"children":[{"label":"Nested One","value":"nested_1"}],"label":"Group 1"}],"event":"getItems"},"event":"sendToPropertyInspector"}
                """, cancellationToken), Times.Once);
        }
    }
}
