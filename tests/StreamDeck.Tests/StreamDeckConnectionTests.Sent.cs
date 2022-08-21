namespace StreamDeck.Tests
{
    using Moq;

    /// <inheritdoc/>
    public partial class StreamDeckConnectionTests
    {
        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.GetGlobalSettingsAsync"/>.
        /// </summary>
        [Test]
        public async Task Send_GetGlobalSettingsAsync()
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.GetGlobalSettingsAsync(token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"{{this.RegistrationParameters.PluginUUID}}","event":"getGlobalSettings"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.LogMessageAsync(string, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task Send_LogMessageAsync()
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.LogMessageAsync("Hello world");

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"payload":{"message":"Hello world"},"event":"logMessage"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.OpenUrlAsync(string, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task Send_OpenUrlAsync()
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.OpenUrlAsync("https://example.com");

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"payload":{"url":"https://example.com"},"event":"openUrl"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetImageAsync(string, string, Target, int?, CancellationToken)"/>.
        /// </summary>
        [Test]
        [TestCase(Target.Both)]
        [TestCase(Target.Hardware)]
        [TestCase(Target.Software)]
        public async Task Send_SetImageAsync(Target target)
        {
            // Arrange
            const string img = "data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==";
            var token = new CancellationToken();

            // Act.
            await this.StreamDeckConnection.SetImageAsync("ABC123", img, target, 0, token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"ABC123","payload":{"image":"{{img}}","state":0,"target":{{(int)target}}},"event":"setImage"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetImageAsync(string, string, Target, int?, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task Send_SetImageAsync_NoState()
        {
            // Arrange
            const string img = "data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==";
            var token = new CancellationToken();

            // Act.
            await this.StreamDeckConnection.SetImageAsync("ABC123", img, Target.Both, cancellationToken: token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"ABC123","payload":{"image":"{{img}}","target":0},"event":"setImage"}
                    """, token),
                Times.Once);
        }
    }
}
