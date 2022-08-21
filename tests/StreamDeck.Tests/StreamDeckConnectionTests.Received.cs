namespace StreamDeck.Tests
{
    using StreamDeck.Events;
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
                    Assert.That(args.Payload?.Application, Is.EqualTo("com.tests.example"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "event": "applicationDidLaunch",
                    "payload" : {
                        "application": "com.tests.example"
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.ApplicationDidTerminate"/>.
        /// </summary>
        [Test]
        public void ApplicationDidTerminate()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.ApplicationDidTerminate += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Event, Is.EqualTo("applicationDidTerminate"));
                    Assert.That(args.Payload?.Application, Is.EqualTo("com.tests.example"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "event": "applicationDidTerminate",
                    "payload" : {
                        "application": "com.tests.example"
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.DeviceDidConnect"/>.
        /// </summary>
        [Test]
        public void DeviceDidConnect()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.DeviceDidConnect += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Event, Is.EqualTo("deviceDidConnect"));
                    Assert.That(args.Device, Is.EqualTo("ABC123"));
                    Assert.That(args.DeviceInfo?.Name, Is.EqualTo("Device Name"));
                    Assert.That(args.DeviceInfo?.Type, Is.EqualTo(Device.StreamDeck));
                    Assert.That(args.DeviceInfo?.Size?.Columns, Is.EqualTo(5));
                    Assert.That(args.DeviceInfo?.Size?.Rows, Is.EqualTo(3));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "event": "deviceDidConnect",
                    "device": "ABC123",
                    "deviceInfo": {
                        "name": "Device Name",
                        "type": 0,
                        "size": {
                            "columns": 5,
                            "rows": 3
                        }
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.DeviceDidDisconnect"/>.
        /// </summary>
        [Test]
        public void DeviceDidDisconnect()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.DeviceDidDisconnect += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Event, Is.EqualTo("deviceDidDisconnect"));
                    Assert.That(args.Device, Is.EqualTo("ABC123"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "event": "deviceDidDisconnect",
                    "device": "ABC123"
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.DidReceiveGlobalSettings"/>.
        /// </summary>
        [Test]
        public void DidReceiveGlobalSettings()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.DidReceiveGlobalSettings += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Event, Is.EqualTo("didReceiveGlobalSettings"));
                    Assert.That(args.Payload?.Settings, Is.Not.Null);
                    Assert.That(args.Payload?.GetSettings<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "event": "didReceiveGlobalSettings",
                    "payload": {
                        "settings": {
                            "name": "Bob Smith"
                        }
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.DidReceiveSettings"/>.
        /// </summary>
        [Test]
        public void DidReceiveSettings()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.DidReceiveSettings += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Device, Is.EqualTo("ZYC789"));
                    Assert.That(args.Event, Is.EqualTo("didReceiveSettings"));
                    Assert.That(args.Payload?.Settings, Is.Not.Null);
                    Assert.That(args.Payload?.GetSettings<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
                    Assert.That(args.Payload?.Coordinates?.Column, Is.EqualTo(3));
                    Assert.That(args.Payload?.Coordinates?.Row, Is.EqualTo(1));
                    Assert.That(args.Payload?.IsInMultiAction, Is.False);
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "device": "ZYC789",
                    "event": "didReceiveSettings",
                    "payload": {
                        "settings": {
                            "name": "Bob Smith"
                        },
                        "coordinates": {
                            "column": 3,
                            "row": 1
                        },
                        "isInMultiAction": false
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Raises <see cref="IWebSocketConnection.MessageReceived"/> when the specified JSON.
        /// </summary>
        /// <param name="json">The JSON message..</param>
        private void Raise(string json)
            => this.WebSocketConnection.Raise(ws => ws.MessageReceived += null, new WebSocketMessageEventArgs(json));

        private class FooSettings
        {
            public string? Name { get; set; }
        }
    }
}
