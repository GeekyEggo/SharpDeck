namespace StreamDeck.Tests
{
    using System.Globalization;
    using StreamDeck.Events;

    /// <summary>
    /// Provides assertions for <see cref="RegistrationParameters"/>.
    /// </summary>
    [TestFixture]
    public class RegistrationParametersTests
    {
        /// <summary>
        /// Asserts <see cref="RegistrationParameters(string[])"/> correctly sets and deserializes all arguments.
        /// </summary>
        [Test]
        public void Construct()
        {
            // Arrange.
            const string infoJson = """
                {
                    "application": {
                        "font": "Calibri",
                        "language": "en",
                        "platform": "windows",
                        "platformVersion": "10.0.22000",
                        "version": "5.3.0.15179"
                    },
                    "colors": {
                        "buttonMouseOverBackgroundColor": "#464646FF",
                        "buttonPressedBackgroundColor": "#303030FF",
                        "buttonPressedBorderColor": "#646464FF",
                        "buttonPressedTextColor": "#969696FF",
                        "highlightColor": "#0078FFFF"
                    },
                    "devicePixelRatio": 2,
                    "devices": [
                        {
                            "id": "B3F7C6D19695AF13B95578F2C29EB037",
                            "name": "Stream Deck XL",
                            "size": {
                                "columns": 8,
                                "rows": 4
                            },
                            "type": 2
                        }
                    ],
                    "plugin": {
                        "uuid": "com.sample.counter",
                        "version": "1.2.0"
                    }
                }
                """;

            // Act.
            var parameters = new RegistrationParameters("-port", "13", "-pluginUUID", "ABCDEF123456", "-registerEvent", "registerPlugin", "-info", infoJson);

            // Assert.
            Assert.Multiple(() =>
            {
                // Basic parameters.
                Assert.That(parameters.Port, Is.EqualTo(13));
                Assert.That(parameters.PluginUUID, Is.EqualTo("ABCDEF123456"));
                Assert.That(parameters.Event, Is.EqualTo("registerPlugin"));

                // Info - Application.
                Assert.That(parameters.Info.Application.Font, Is.EqualTo("Calibri"));
                Assert.That(parameters.Info.Application.Language, Is.EqualTo(CultureInfo.GetCultureInfo("en")));
                Assert.That(parameters.Info.Application.Platform, Is.EqualTo("windows"));
                Assert.That(parameters.Info.Application.PlatformVersion, Is.EqualTo(new Version(10, 0, 22000)));
                Assert.That(parameters.Info.Application.Version, Is.EqualTo(new Version(5, 3, 0, 15179)));

                // Info - Colors
                Assert.That(parameters.Info.Colors.ButtonMouseOverBackgroundColor, Is.EqualTo("#464646FF"));
                Assert.That(parameters.Info.Colors.ButtonPressedBackgroundColor, Is.EqualTo("#303030FF"));
                Assert.That(parameters.Info.Colors.ButtonPressedBorderColor, Is.EqualTo("#646464FF"));
                Assert.That(parameters.Info.Colors.ButtonPressedTextColor, Is.EqualTo("#969696FF"));
                Assert.That(parameters.Info.Colors.HighlightColor, Is.EqualTo("#0078FFFF"));

                // Info - Devices.
                Assert.That(parameters.Info.DevicePixelRatio, Is.EqualTo(2));
                Assert.That(parameters.Info.Devices.Length, Is.EqualTo(1));
                Assert.That(parameters.Info.Devices[0].Id, Is.EqualTo("B3F7C6D19695AF13B95578F2C29EB037"));
                Assert.That(parameters.Info.Devices[0].Name, Is.EqualTo("Stream Deck XL"));
                Assert.That(parameters.Info.Devices[0].Size.Columns, Is.EqualTo(8));
                Assert.That(parameters.Info.Devices[0].Size.Rows, Is.EqualTo(4));
                Assert.That(parameters.Info.Devices[0].Type, Is.EqualTo(Device.StreamDeckXL));

                // Info - Plugin.
                Assert.That(parameters.Info.Plugin.UUID, Is.EqualTo("com.sample.counter"));
                Assert.That(parameters.Info.Plugin.Version, Is.EqualTo(new Version(1, 2, 0)));
            });
        }
    }
}
