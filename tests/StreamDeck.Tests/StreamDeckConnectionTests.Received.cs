namespace StreamDeck.Tests
{
    using StreamDeck.Events;
    using StreamDeck.Extensions;
    using StreamDeck.Net;
    using StreamDeck.Tests.Helpers;

    /// <inheritdoc/>
    public partial class StreamDeckConnectionTests
    {
        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.ApplicationDidLaunch"/>.
        /// </summary>
        [Test]
        public void Receive_ApplicationDidLaunch()
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
                    Assert.That(args.Payload.Application, Is.EqualTo("com.tests.example"));
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
        public void Receive_ApplicationDidTerminate()
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
                    Assert.That(args.Payload.Application, Is.EqualTo("com.tests.example"));
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
        public void Receive_DeviceDidConnect()
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
                    Assert.That(args.DeviceInfo.Name, Is.EqualTo("Device Name"));
                    Assert.That(args.DeviceInfo.Type, Is.EqualTo(Device.StreamDeck));
                    Assert.That(args.DeviceInfo.Size.Columns, Is.EqualTo(5));
                    Assert.That(args.DeviceInfo.Size.Rows, Is.EqualTo(3));
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
        public void Receive_DeviceDidDisconnect()
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
        public void Receive_DidReceiveGlobalSettings()
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
                    Assert.That(args.Payload.Settings, Is.Not.Null);
                    Assert.That(args.Payload.GetSettings<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
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
        public void Receive_DidReceiveSettings()
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
                    Assert.That(args.Payload.Settings, Is.Not.Null);
                    Assert.That(args.Payload.GetSettings<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
                    Assert.That(args.Payload.Coordinates.Column, Is.EqualTo(3));
                    Assert.That(args.Payload.Coordinates.Row, Is.EqualTo(1));
                    Assert.That(args.Payload.IsInMultiAction, Is.False);
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
                        "coordinates": {
                            "column": 3,
                            "row": 1
                        },
                        "settings": {
                            "name": "Bob Smith"
                        },
                        "isInMultiAction": false
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.KeyDown"/>.
        /// </summary>
        [Test]
        public void Receive_KeyDown()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.KeyDown += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Device, Is.EqualTo("ZYC789"));
                    Assert.That(args.Event, Is.EqualTo("keyDown"));
                    Assert.That(args.Payload.Settings, Is.Not.Null);
                    Assert.That(args.Payload.GetSettings<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
                    Assert.That(args.Payload.Coordinates.Column, Is.EqualTo(3));
                    Assert.That(args.Payload.Coordinates.Row, Is.EqualTo(1));
                    Assert.That(args.Payload.IsInMultiAction, Is.True);
                    Assert.That(args.Payload.State, Is.EqualTo(0));
                    Assert.That(args.Payload.UserDesiredState, Is.EqualTo(1));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "device": "ZYC789",
                    "event": "keyDown",
                    "payload": {
                        "coordinates": {
                            "column": 3,
                            "row": 1
                        },
                        "isInMultiAction": true,
                        "settings": {
                            "name": "Bob Smith"
                        },
                        "state": 0,
                        "userDesiredState": 1
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.KeyDown"/> handles no <see cref="KeyPayload.UserDesiredState"/>, no <see cref="ActionPayload.State"/>, and no <see cref="SettingsPayload.Settings"/>.
        /// </summary>
        [Test]
        public void Receive_KeyDown_NoStateOrSettings()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.KeyDown += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Device, Is.EqualTo("ZYC789"));
                    Assert.That(args.Event, Is.EqualTo("keyDown"));
                    Assert.That(args.Payload.Settings, Is.Not.Null);
                    Assert.That(args.Payload.Coordinates.Column, Is.EqualTo(3));
                    Assert.That(args.Payload.Coordinates.Row, Is.EqualTo(1));
                    Assert.That(args.Payload.IsInMultiAction, Is.True);
                    Assert.That(args.Payload.State, Is.Null);
                    Assert.That(args.Payload.UserDesiredState, Is.Null);
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "device": "ZYC789",
                    "event": "keyDown",
                    "payload": {
                        "coordinates": {
                            "column": 3,
                            "row": 1
                        },
                        "isInMultiAction": true,
                        "settings": {}
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }



        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.KeyUp"/>.
        /// </summary>
        [Test]
        public void Receive_KeyUp()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.KeyUp += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Device, Is.EqualTo("ZYC789"));
                    Assert.That(args.Event, Is.EqualTo("keyUp"));
                    Assert.That(args.Payload.Settings, Is.Not.Null);
                    Assert.That(args.Payload.GetSettings<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
                    Assert.That(args.Payload.Coordinates.Column, Is.EqualTo(1));
                    Assert.That(args.Payload.Coordinates.Row, Is.EqualTo(2));
                    Assert.That(args.Payload.IsInMultiAction, Is.True);
                    Assert.That(args.Payload.State, Is.EqualTo(1));
                    Assert.That(args.Payload.UserDesiredState, Is.EqualTo(0));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "device": "ZYC789",
                    "event": "keyUp",
                    "payload": {
                        "coordinates": {
                            "column": 1,
                            "row": 2
                        },
                        "isInMultiAction": true,
                        "settings": {
                            "name": "Bob Smith"
                        },
                        "state": 1,
                        "userDesiredState": 0
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.PropertyInspectorDidAppear"/>.
        /// </summary>
        [Test]
        public void Receive_PropertyInspectorDidAppear()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.PropertyInspectorDidAppear += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Device, Is.EqualTo("ZYC789"));
                    Assert.That(args.Event, Is.EqualTo("propertyInspectorDidAppear"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "device": "ZYC789",
                    "event": "propertyInspectorDidAppear"
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.PropertyInspectorDidDisappear"/>.
        /// </summary>
        [Test]
        public void Receive_PropertyInspectorDidDisappear()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.PropertyInspectorDidDisappear += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Device, Is.EqualTo("ZYC789"));
                    Assert.That(args.Event, Is.EqualTo("propertyInspectorDidDisappear"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "device": "ZYC789",
                    "event": "propertyInspectorDidDisappear"
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SendToPlugin"/>.
        /// </summary>
        [Test]
        public void Receive_SendToPlugin()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.SendToPlugin += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Event, Is.EqualTo("sendToPlugin"));
                    Assert.That(args.Payload, Is.Not.Null);
                    Assert.That(args.GetPayload<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "event": "sendToPlugin",
                    "payload": {
                        "name": "Bob Smith"
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.SystemDidWakeUp"/>.
        /// </summary>
        [Test]
        public void Receive_SystemDidWakeUp()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.SystemDidWakeUp += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));
                    Assert.That(args.Event, Is.EqualTo("systemDidWakeUp"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "event": "systemDidWakeUp"
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.TitleParametersDidChange"/>.
        /// </summary>
        [Test]
        public void Receive_TitleParametersDidChange()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.TitleParametersDidChange += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Device, Is.EqualTo("ZYC789"));
                    Assert.That(args.Event, Is.EqualTo("titleParametersDidChange"));
                    Assert.That(args.Payload.Coordinates.Column, Is.EqualTo(5));
                    Assert.That(args.Payload.Coordinates.Row, Is.EqualTo(2));
                    Assert.That(args.Payload.Settings, Is.Not.Null);
                    Assert.That(args.Payload.GetSettings<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
                    Assert.That(args.Payload.State, Is.EqualTo(1));
                    Assert.That(args.Payload.Title, Is.EqualTo("Hello World"));
                    Assert.That(args.Payload.TitleParameters.FontFamily, Is.EqualTo("Calibri"));
                    Assert.That(args.Payload.TitleParameters.FontSize, Is.EqualTo(12));
                    Assert.That(args.Payload.TitleParameters.FontStyle, Is.EqualTo("Bold"));
                    Assert.That(args.Payload.TitleParameters.FontUnderline, Is.EqualTo(false));
                    Assert.That(args.Payload.TitleParameters.ShowTitle, Is.EqualTo(true));
                    Assert.That(args.Payload.TitleParameters.TitleAlignment, Is.EqualTo("bottom"));
                    Assert.That(args.Payload.TitleParameters.TitleColor, Is.EqualTo("#ffff00"));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "device": "ZYC789",
                    "event": "titleParametersDidChange",
                    "payload": {
                        "coordinates": {
                            "column": 5,
                            "row": 2
                        },
                        "settings": {
                            "name": "Bob Smith"
                        },
                        "state": 1,
                        "title": "Hello World",
                        "titleParameters": {
                            "fontFamily": "Calibri",
                            "fontSize": 12,
                            "fontStyle": "Bold",
                            "fontUnderline": false,
                            "showTitle": true,
                            "titleAlignment": "bottom",
                            "titleColor": "#ffff00"
                        }
                    }
                }
                """);

            // Assert.
            Assert.That(didHandle, Is.True, "Event was not raised.");
        }

        /// <summary>
        /// Asserts <see cref="StreamDeckConnection.WillAppear"/>.
        /// </summary>
        [Test]
        public void Receive_WillAppear()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.WillAppear += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Device, Is.EqualTo("ZYC789"));
                    Assert.That(args.Event, Is.EqualTo("willAppear"));
                    Assert.That(args.Payload.Coordinates.Column, Is.EqualTo(3));
                    Assert.That(args.Payload.Coordinates.Row, Is.EqualTo(1));
                    Assert.That(args.Payload.IsInMultiAction, Is.False);
                    Assert.That(args.Payload.Settings, Is.Not.Null);
                    Assert.That(args.Payload.GetSettings<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
                    Assert.That(args.Payload.State, Is.Null);
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "device": "ZYC789",
                    "event": "willAppear",
                    "payload": {
                        "coordinates": {
                            "column": 3,
                            "row": 1
                        },
                        "isInMultiAction": false,
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
        /// Asserts <see cref="StreamDeckConnection.WillDisappear"/>.
        /// </summary>
        [Test]
        public void Receive_WillDisappear()
        {
            // Arrange.
            var didHandle = false;
            this.StreamDeckConnection.WillAppear += (sender, args) =>
            {
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(sender, Is.EqualTo(this.StreamDeckConnection));

                    Assert.That(args.Action, Is.EqualTo("com.tests.example.action"));
                    Assert.That(args.Context, Is.EqualTo("ABC123"));
                    Assert.That(args.Device, Is.EqualTo("ZYC789"));
                    Assert.That(args.Event, Is.EqualTo("willAppear"));
                    Assert.That(args.Payload.Coordinates.Column, Is.EqualTo(1));
                    Assert.That(args.Payload.Coordinates.Row, Is.EqualTo(2));
                    Assert.That(args.Payload.IsInMultiAction, Is.True);
                    Assert.That(args.Payload.Settings, Is.Not.Null);
                    Assert.That(args.Payload.GetSettings<FooSettings>()?.Name, Is.EqualTo("Bob Smith"));
                    Assert.That(args.Payload.State, Is.EqualTo(0));
                });

                didHandle = true;
            };

            // Act.
            this.Raise("""
                {
                    "action": "com.tests.example.action",
                    "context": "ABC123",
                    "device": "ZYC789",
                    "event": "willAppear",
                    "payload": {
                        "coordinates": {
                            "column": 1,
                            "row": 2
                        },
                        "isInMultiAction": true,
                        "settings": {
                            "name": "Bob Smith"
                        },
                        "state": 0
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
    }
}
