namespace StreamDeck.Tests
{
    using System.Text.Json.Serialization.Metadata;
    using Moq;
    using StreamDeck.Payloads;
    using StreamDeck.Tests.Helpers;
    using StreamDeck.Tests.Serialization;

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
        /// Asserts <see cref="StreamDeckConnection.GetSettingsAsync(string, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task Send_GetSettingsAsync()
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.GetSettingsAsync("ABC123", token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync("""
                    {"context":"ABC123","event":"getSettings"}
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
                ws => ws.SendAsync("""
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
                ws => ws.SendAsync("""
                    {"payload":{"url":"https://example.com"},"event":"openUrl"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SendToPropertyInspectorAsync{TPayload}(string, string, TPayload, JsonTypeInfo{TPayload}?, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task Send_SendToPropertyInspectorAsync()
        {
            // Arrange
            var payload = new FooSettings { Name = "Bob Smith" };
            var token = new CancellationToken();

            // Act.
            await this.StreamDeckConnection.SendToPropertyInspectorAsync("ABC123", "com.tests.example.action", payload, cancellationToken: token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync("""
                    {"action":"com.tests.example.action","context":"ABC123","payload":{"name":"Bob Smith"},"event":"sendToPropertyInspector"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SendToPropertyInspectorAsync{TPayload}(string, string, TPayload, JsonTypeInfo{TPayload}?, CancellationToken)"/> with a custom <see cref="JsonTypeInfo{T}"/>.
        /// </summary>
        [Test]
        public async Task Send_SendToPropertyInspectorAsync_WithJsonTypeInfo()
        {
            // Arrange
            var payload = new FooSettings { Name = "Bob Smith" };
            var token = new CancellationToken();

            // Act.
            await this.StreamDeckConnection.SendToPropertyInspectorAsync("ABC123", "com.tests.example.action", payload, TestJsonContext.Default.FooSettings, token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync("""
                    {"action":"com.tests.example.action","context":"ABC123","payload":{"Name":"Bob Smith"},"event":"sendToPropertyInspector"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetGlobalSettingsAsync{TSettings}(TSettings, JsonTypeInfo{TSettings}?, CancellationToken)"/>
        /// </summary>
        [Test]
        public async Task Send_SetGlobalSettingsAsync()
        {
            // Arrange
            var settings = new FooSettings { Name = "Bob Smith" };
            var token = new CancellationToken();

            // Act
            await this.StreamDeckConnection.SetGlobalSettingsAsync(settings, cancellationToken: token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"{{this.RegistrationParameters.PluginUUID}}","payload":{"name":"Bob Smith"},"event":"setGlobalSettings"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetGlobalSettingsAsync{TSettings}(TSettings, JsonTypeInfo{TSettings}?, CancellationToken)"/> with a custom <see cref="JsonTypeInfo{T}"/>.
        /// </summary>
        [Test]
        public async Task Send_SetGlobalSettingsAsync_WithJsonTypeInfo()
        {
            // Arrange
            var settings = new FooSettings { Name = "Bob Smith" };
            var token = new CancellationToken();

            // Act
            await this.StreamDeckConnection.SetGlobalSettingsAsync(settings, TestJsonContext.Default.FooSettings, token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"{{this.RegistrationParameters.PluginUUID}}","payload":{"Name":"Bob Smith"},"event":"setGlobalSettings"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetImageAsync(string, string, Target, int?, CancellationToken)"/>.
        /// </summary>
        /// <param name="target">The <see cref="Target"/> supplied to <see cref="StreamDeckConnection.SetImageAsync(string, string, Target, int?, CancellationToken)"/>.</param>
        /// <param name="target">The state supplied to <see cref="StreamDeckConnection.SetImageAsync(string, string, Target, int?, CancellationToken)"/>.</param>
        [Test]
        [TestCase(Target.Both, 1)]
        [TestCase(Target.Hardware, 0)]
        [TestCase(Target.Software, 1)]
        public async Task Send_SetImageAsync(Target target, int state)
        {
            // Arrange
            const string img = "data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==";
            var token = new CancellationToken();

            // Act.
            await this.StreamDeckConnection.SetImageAsync("ABC123", img, target, state, token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"ABC123","payload":{"image":"{{img}}","state":{{state}},"target":{{(int)target}}},"event":"setImage"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetImageAsync(string, string, Target, int?, CancellationToken)"/> ignores <see cref="TargetPayload.State"/> when <c>null</c>.
        /// </summary>
        /// <param name="target">The <see cref="Target"/> supplied to <see cref="StreamDeckConnection.SetImageAsync(string, string, Target, int?, CancellationToken)"/>.</param>
        [Test]
        [TestCase(Target.Both)]
        [TestCase(Target.Hardware)]
        [TestCase(Target.Software)]
        public async Task Send_SetImageAsync_NoState(Target target)
        {
            // Arrange
            const string img = "data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==";
            var token = new CancellationToken();

            // Act.
            await this.StreamDeckConnection.SetImageAsync("ABC123", img, target, cancellationToken: token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"ABC123","payload":{"image":"{{img}}","target":{{(int)target}}},"event":"setImage"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetSettingsAsync{TSettings}(string, TSettings, JsonTypeInfo{TSettings}?, CancellationToken)"/>
        /// </summary>
        [Test]
        public async Task Send_SetSettingsAsync()
        {
            // Arrange
            var settings = new FooSettings { Name = "Bob Smith" };
            var token = new CancellationToken();

            // Act
            await this.StreamDeckConnection.SetSettingsAsync("ABC123", settings, cancellationToken: token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync("""
                    {"context":"ABC123","payload":{"name":"Bob Smith"},"event":"setSettings"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetSettingsAsync{TSettings}(string, TSettings, JsonTypeInfo{TSettings}?, CancellationToken)"/> with a custom <see cref="JsonTypeInfo{T}"/>.
        /// </summary>
        [Test]
        public async Task Send_SetSettingsAsync_WithJsonTypeInfo()
        {
            // Arrange
            var settings = new FooSettings { Name = "Bob Smith" };
            var token = new CancellationToken();

            // Act
            await this.StreamDeckConnection.SetSettingsAsync("ABC123", settings, TestJsonContext.Default.FooSettings, token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync("""
                    {"context":"ABC123","payload":{"Name":"Bob Smith"},"event":"setSettings"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetStateAsync(string, int, CancellationToken).
        /// </summary>
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public async Task Send_SetStateAsync(int state)
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.SetStateAsync("ABC123", state, token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"ABC123","payload":{"state":{{state}}},"event":"setState"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetTitleAsync(string, string, Target, int?, CancellationToken)"/>.
        /// </summary>
        /// <param name="target">The <see cref="Target"/> supplied to <see cref="StreamDeckConnection.SetTitleAsync(string, string, Target, int?, CancellationToken)"/>.</param>
        /// <param name="state">The state supplied to <see cref="StreamDeckConnection.SetTitleAsync(string, string, Target, int?, CancellationToken)"/>.</param>
        [Test]
        [TestCase(Target.Both, 1)]
        [TestCase(Target.Hardware, 0)]
        [TestCase(Target.Software, 1)]
        public async Task Send_SetTitleAsync(Target target, int state)
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.SetTitleAsync("ABC123", "Hello world", target, state, token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"ABC123","payload":{"title":"Hello world","state":{{state}},"target":{{(int)target}}},"event":"setTitle"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SetTitleAsync(string, string, Target, int?, CancellationToken)"/> ignores <see cref="TargetPayload.State"/> when <c>null</c>.
        /// </summary>
        /// <param name="target">The <see cref="Target"/> supplied to <see cref="StreamDeckConnection.SetTitleAsync(string, string, Target, int?, CancellationToken)"/>.</param>
        [Test]
        [TestCase(Target.Both)]
        [TestCase(Target.Hardware)]
        [TestCase(Target.Software)]
        public async Task Send_SetTitleAsync_NoState(Target target)
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.SetTitleAsync("ABC123", "Hello world", target, cancellationToken: token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"context":"ABC123","payload":{"title":"Hello world","target":{{(int)target}}},"event":"setTitle"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.ShowAlertAsync(string, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task Send_ShowAlertAsync()
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.ShowAlertAsync("ABC123", token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync("""
                    {"context":"ABC123","event":"showAlert"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.ShowOkAsync(string, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task Send_ShowOkAsync()
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.ShowOkAsync("ABC123", token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync("""
                    {"context":"ABC123","event":"showOk"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SwitchToProfileAsync(string, string, string, CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task Send_SwitchToProfileAsync()
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.SwitchToProfileAsync("ABC123", "Main Profile", token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"device":"ABC123","context":"{{this.RegistrationParameters.PluginUUID}}","payload":{"profile":"Main Profile"},"event":"switchToProfile"}
                    """, token),
                Times.Once);
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SwitchToProfileAsync(string, string, string, CancellationToken)"/> ignore <see cref="SwitchToProfilePayload.Profile"/> when <c>null</c>.
        /// </summary>
        [Test]
        public async Task Send_SwitchToProfileAsync_NoProfile()
        {
            // Arrange, act.
            var token = new CancellationToken();
            await this.StreamDeckConnection.SwitchToProfileAsync("ABC123", cancellationToken: token);

            // Assert
            this.WebSocketConnection.Verify(
                ws => ws.SendAsync($$"""
                    {"device":"ABC123","context":"{{this.RegistrationParameters.PluginUUID}}","payload":{},"event":"switchToProfile"}
                    """, token),
                Times.Once);
        }
    }
}
