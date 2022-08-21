namespace StreamDeck.Tests
{
    using StreamDeck.Net;

    /// <inheritdoc/>
    public partial class StreamDeckConnectionTests
    {
        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.ApplicationDidLaunch"/>.
        /// </summary>
        [Test]
        public void ApplicationDidLaunch()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.ApplicationDidLaunch += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Event, Is.EqualTo("applicationDidLaunch"));
                    Assert.That(args.Payload?.Application, Is.EqualTo("com.example.counter"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "event": "applicationDidLaunch",
                    "payload" : {
                        "application": "com.example.counter"
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True);
        }

        /// <summary>
        /// Raises <see cref="IWebSocketConnection.MessageReceived"/> when the specified JSON.
        /// </summary>
        /// <param name="json">The JSON message..</param>
        private void Raise(string json)
            => this.WebSocketConnection.Raise(ws => ws.MessageReceived += null, new WebSocketMessageEventArgs(json));
    }
}
