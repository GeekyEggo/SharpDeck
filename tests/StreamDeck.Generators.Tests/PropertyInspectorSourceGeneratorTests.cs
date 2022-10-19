namespace StreamDeck.Generators.Tests
{
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Tests.Helpers;

    [TestFixture]
    internal class PropertyInspectorSourceGeneratorTests
    {
        private const string SDPI_COMPONENTS_SRC = "https://cdn.jsdelivr.net/gh/geekyeggo/sdpi-components@v2/dist/sdpi-components.js";
        [Test]
        public void Textfield()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                using StreamDeck;
                using StreamDeck.PropertyInspectors;

                [Action(PropertyInspectorType = typeof(Settings), UUID = "com.tests.example.action"]
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
                    HintName: "pi\\com.tests.example.action.g.html",
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
    }
}
