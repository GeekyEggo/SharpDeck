namespace StreamDeck.Generators.Tests
{
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Tests.Helpers;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides assertions for <see cref="PropertyInspectorSourceGenerator"/>.
    /// </summary>
    [TestFixture]
    internal class PropertyInspectorSourceGeneratorTests
    {
        /// <summary>
        /// The sdpi-components source location.
        /// </summary>
        private const string SDPI_COMPONENTS_SRC = "https://cdn.jsdelivr.net/gh/geekyeggo/sdpi-components@v2/dist/sdpi-components.js";

        /// <summary>
        /// The source text of an empty property inspector.
        /// </summary>
        private const string EMPTY_PI = $"""
            <!DOCTYPE html>
            <html>
                <head lang="en">
                    <meta charset="utf-8" />
                    <script src="{SDPI_COMPONENTS_SRC}"></script>
                </head>
                <body></body>
            </html>

            """;

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="CalendarAttribute"/> component correctly.
        /// </summary>
        [Test]
        [TestCase("CalendarType.Date", "date")]
        [TestCase("CalendarType.DateTimeLocal", "datetime-local")]
        [TestCase("CalendarType.Month", "month")]
        [TestCase("CalendarType.Week", "week")]
        [TestCase("CalendarType.Time", "time")]
        public void GenerateCalendar(string calendarType, string expectedType)
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            var sourceText = $$"""
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Calendar(
                        IsDisabled = true,
                        IsGlobal = true,
                        Label = "Calendar",
                        Max = "2022-12-31",
                        Min = "2022-12-25",
                        Setting = "calendar"
                        Step = 1,
                        Type = {{calendarType}})]
                    public string Description { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item label="Calendar">
                                <sdpi-calendar disabled global max="2022-12-31" min="2022-12-25" setting="calendar" step="1" type="{expectedType}"></sdpi-calendar>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="CheckboxAttribute"/> component correctly.
        /// </summary>
        [Test]
        public void GenerateCheckbox()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            var sourceText = $$"""
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Checkbox(
                        IsDisabled = true,
                        IsGlobal = true,
                        Label = "Checkbox",
                        CheckboxLabel = "True?",
                        Setting = "is_true")]
                    public bool IsTrue { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item label="Checkbox">
                                <sdpi-checkbox disabled global label="True?" setting="is_true"></sdpi-checkbox>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="ColorAttribute"/> component correctly.
        /// </summary>
        [Test]
        public void GenerateColor()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            var sourceText = $$"""
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Color(
                        IsDisabled = true,
                        IsGlobal = true,
                        Label = "Color"
                        Setting = "color")]
                    public string Color { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item label="Color">
                                <sdpi-color disabled global setting="color"></sdpi-color>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="FileAttribute"/> component correctly.
        /// </summary>
        [Test]
        public void GenerateFile()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            var sourceText = $$"""
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [File(
                        IsDisabled = true,
                        IsGlobal = true,
                        Label = "File"
                        Accept = "image/png, image/jpeg",
                        ButtonLabel = "Choose",
                        Setting = "avatar")]
                    public string FilePath { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item label="File">
                                <sdpi-file disabled global accept="image/png, image/jpeg" label="Choose" setting="avatar"></sdpi-file>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="PasswordAttribute"/> component correctly.
        /// </summary>
        [Test]
        public void GeneratePassword()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Password(
                        IsDisabled = true,
                        IsGlobal = true,
                        Label = "Password",
                        MaxLength = 100,
                        Setting = "password")]
                    public string Password { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item label="Password">
                                <sdpi-password disabled global maxlength="100" setting="password"></sdpi-password>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="RangeAttribute"/> component correctly.
        /// </summary>
        [Test]
        public void GenerateRange()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Range(
                        IsDisabled = true,
                        IsGlobal = true,
                        Max = 100,
                        Min = 0,
                        Label = "Volume",
                        Setting = "volume",
                        ShowLabels = true,
                        Step = 5)]
                    public string Volume { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item label="Volume">
                                <sdpi-range disabled global max="100" min="0" setting="volume" showlabels step="5"></sdpi-range>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="SelectAttribute"/> component correctly.
        /// </summary>
        [Test]
        public void GenerateSelect()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Select(
                        IsDisabled = true,
                        IsGlobal = true,
                        Label = "Select"
                        DataSource = "GetNumbers",
                        HotReload = true,
                        Loading = "Loading...",
                        Placeholder = "Please select",
                        Setting = "number"
                        ValueType = InputValueType.Number)]
                    [Option(Value = "1", Label = "One", Group = "Prime")]
                    [Option(Value = "2", Label = "Two", Group = "Prime")]
                    [Option(Value = "3", Label = "Three", IsDisabled = true, Group = "Prime")]
                    [Option(Value = "4", Label = "Four")]
                    public int Number { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item label="Select">
                                <sdpi-select disabled global datasource="GetNumbers" hot-reload loading="Loading..." placeholder="Please select" setting="number" value-type="number">
                                    <optgroup label="Prime">
                                        <option value="1">One</option>
                                        <option value="2">Two</option>
                                        <option value="3" disabled>Three</option>
                                    </optgroup>
                                    <option value="4">Four</option>
                                </sdpi-select>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="TextareaAttribute"/> component correctly.
        /// </summary>
        [Test]
        public void GenerateTextarea()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Textarea(
                        IsDisabled = true,
                        IsGlobal = true,
                        Label = "Description",
                        MaxLength = 250,
                        Rows = 3,
                        Setting = "description"
                        ShowLength = true)]
                    public string Description { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item label="Description">
                                <sdpi-textarea disabled global maxlength="250" rows="3" setting="description" showlength></sdpi-textarea>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="TextfieldAttribute"/> component correctly.
        /// </summary>
        [Test]
        public void GenerateTextfield()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Textfield(
                        IsDisabled = true,
                        IsGlobal = true,
                        IsRequired = true,
                        Label = "Name",
                        MaxLength = 100,
                        Pattern = "[A-Za-z]",
                        Placeholder = "Please enter your name",
                        Setting = "name")]
                    public string Name { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item label="Name">
                                <sdpi-textfield disabled global required maxlength="100" pattern="[A-Za-z]" placeholder="Please enter your name" setting="name"></sdpi-textfield>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts the <see cref="InputAttribute.Setting"/> is inferred from the name of the property.
        /// </summary>
        [Test]
        public void InferSetting_FromPropertyName()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Checkbox]
                    public bool IsChecked { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item>
                                <sdpi-checkbox setting="isChecked"></sdpi-checkbox>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts the <see cref="InputAttribute.Setting"/> is inferred from the <see cref="T:System.Text.Json.Serialization.JsonPropertyNameAttribute"/> when defined.
        /// </summary>
        [Test]
        public void InferSetting_FromJsonPropertyName()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            var sourceText = $$"""
                using StreamDeck;
                using StreamDeck.PropertyInspectors;
                using System.Text.Json.Serialization;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Checkbox]
                    [JsonPropertyName("IS_CHECKED")]
                    public bool IsChecked { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item>
                                <sdpi-checkbox setting="IS_CHECKED"></sdpi-checkbox>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts the <see cref="InputAttribute.Setting"/> is not written when it is explicitly defined.
        /// </summary>
        [TestCase(null)]
        [TestCase("\"\"")]
        public void InferSetting_WhenPropertyInspectorDefined(string setting)
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            var sourceText = $$"""
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings
                {
                    [Checkbox(Setting = {{setting}}]
                    public bool IsChecked { get; set; }
                }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\action.g.html",
                    SourceText: $"""
                    <!DOCTYPE html>
                    <html>
                        <head lang="en">
                            <meta charset="utf-8" />
                            <script src="{SDPI_COMPONENTS_SRC}"></script>
                        </head>
                        <body>
                            <sdpi-item>
                                <sdpi-checkbox></sdpi-checkbox>
                            </sdpi-item>
                        </body>
                    </html>

                    """
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> writes the property inspector to the <see cref="ActionAttribute.PropertyInspectorPath"/> when defined.
        /// </summary>
        [Test]
        public void WriteTo_PropertyInspectorPath()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(
                    PropertyInspectorPath = @"other\pi\location.html",
                    PropertyInspectorType = typeof(Settings))]
                public class Action { }

                public class Settings { }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"other\pi\location.html",
                    SourceText: EMPTY_PI
                ));
        }

        /// <summary>
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> writes the property inspector to the <see cref="ActionAttribute.Name"/> when the <see cref="ActionAttribute.PropertyInspectorPath"/> is not defined.
        /// </summary>
        [Test]
        public void WriteTo_Name()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(
                    PropertyInspectorType = typeof(Settings),
                    Name = "my/action")]
                public class Action { }

                public class Settings { }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\myaction.g.html",
                    SourceText: EMPTY_PI
                ));
        }
    }
}
