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
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> generates a <see cref="TextfieldAttribute"/> component correctly.
        /// </summary>
        [Test]
        public void Textfield()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(
                    PropertyInspectorType = typeof(Settings),
                    UUID = "com.user.product.action")]
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
                    HintName: @"pi\com.user.product.action.g.html",
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
        /// Asserts <see cref="PropertyInspectorSourceGenerator"/> writes the property inspector to the <see cref="ActionAttribute.UUID"/> when the <see cref="ActionAttribute.PropertyInspectorPath"/> is not defined.
        /// </summary>
        [Test]
        public void WriteTo_UUID()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(
                    PropertyInspectorType = typeof(Settings),
                    UUID = "com.user.product.action")]
                public class Action { }

                public class Settings { }
            """;

            // Act
            SourceGeneratorTests.Run(new PropertyInspectorSourceGenerator(fileSystem.Object), sourceText);

            // Assert.
            SourceGeneratorTests.VerifyFiles(
                fileSystem,
                (
                    HintName: @"pi\com.user.product.action.g.html",
                    SourceText: EMPTY_PI
                ));
        }
    }
}
