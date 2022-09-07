namespace StreamDeck.Generators.Tests
{
    using System.Text;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="ManifestSourceGenerator"/>.
    /// </summary>
    [TestFixture]
    public class ManifestSourceGeneratorTests
    {
        /// <summary>
        /// Assert generating a manifest from the assembly attribute.
        /// </summary>
        [Test]
        public void GenerateManifest()
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
                [assembly: StreamDeck.Manifest(
                    Author = "Bob Smith",
                    Category = "Tests",
                    CategoryIcon = "Images/Category/Icon",
                    CodePath = "default.exe",
                    CodePathWin = "windows.exe",
                    CodePathMac = "com.tests.example",
                    Description = "Mock manifest",
                    Icon = "Images/Icon",
                    Name = "Example",
                    PropertyInspectorPath = "pi/default.html",
                    DefaultWindowSize = new[] { 100, 200 },
                    Url = "https://example.com",
                    Version = "1.2.3",
                    OSWindowsMinimumVersion = "11",
                    OSMacMinimumVersion = "10.11",
                    SoftwareMinimumVersion = "5.0",
                    ApplicationsToMonitorWin = new[] { "notepad.exe", "chrome.exe" },
                    ApplicationsToMonitorMac = new[] { "com.apple.mail", "com.apple.safari" })]
                """;

            // Act.
            SourceGeneratorTests.Run(
                new ManifestSourceGenerator(fileSystem.Object),
                sourceText);

            // Assert.
            const string json = $$"""
                {
                    "ApplicationsToMonitor": {
                        "mac": [
                            "com.apple.mail",
                            "com.apple.safari"
                        ],
                        "windows": [
                            "notepad.exe",
                            "chrome.exe"
                        ]
                    },
                    "Author": "Bob Smith",
                    "Category": "Tests",
                    "CategoryIcon": "Images/Category/Icon",
                    "CodePath": "default.exe",
                    "CodePathMac": "com.tests.example",
                    "CodePathWin": "windows.exe",
                    "DefaultWindowSize": [
                        100,
                        200
                    ],
                    "Description": "Mock manifest",
                    "Icon": "Images/Icon",
                    "Name": "Example",
                    "OS": [
                        {
                            "MinimumVersion": "10.11",
                            "Platform": "mac"
                        },
                        {
                            "MinimumVersion": "11",
                            "Platform": "windows"
                        }
                    ],
                    "PropertyInspectorPath": "pi/default.html",
                    "SDKVersion": 2,
                    "Software": {
                        "MinimumVersion": "5.0"
                    },
                    "URL": "https://example.com",
                    "Version": "1.2.3"
                }
                """;

            fileSystem.Verify(f => f.WriteAllText(@"C:\temp\manifest.json", json, Encoding.UTF8), Times.Once);
        }
    }
}
