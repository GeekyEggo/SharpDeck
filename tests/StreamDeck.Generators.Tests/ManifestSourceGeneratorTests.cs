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
        /// Asserts <see cref="ManifestSourceGenerator"/> generates a manifest file.
        /// </summary>
        [Test]
        public void WriteManifest()
        {
            // Arrange.
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

            // Act, assert.
            Verify(sourceText, json);
        }

        /// <summary>
        /// Asserts <see cref="ManifestSourceGenerator"/> reads the assembly information, populating as much as possible.
        /// </summary>
        [Test]
        public void ReadAssemblyInfo()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;
                using System.Reflection;

                [assembly: Manifest]
                [assembly: AssemblyDescription("Hello world, this is a test")]
                [assembly: AssemblyCompany("Bob Smith")]
                [assembly: AssemblyVersion("12.34.56")]
                """;

            const string json = $$"""
                {
                    "Author": "Bob Smith",
                    "CodePath": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.exe",
                    "Description": "Hello world, this is a test",
                    "Icon": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.png",
                    "Name": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}",
                    "OS": [
                        {
                            "MinimumVersion": "10",
                            "Platform": "windows"
                        }
                    ],
                    "SDKVersion": 2,
                    "Software": {
                        "MinimumVersion": "5.0"
                    },
                    "Version": "12.34.56"
                }
                """;

            // Act, assert.
            Verify(sourceText, json);
        }

        /// <summary>
        /// Asserts <see cref="ManifestSourceGenerator"/> adds profiles to the manifest.
        /// </summary>
        [Test]
        public void WriteProfiles()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]
                [assembly: Profile("Profile 1", Device.StreamDeck)]
                [assembly: Profile("Profile 2", Device.StreamDeckXL, Readonly = true, DontAutoSwitchWhenInstalled = true)]
                [assembly: Profile("Profile 3", Device.StreamDeckPedal, Readonly = false, DontAutoSwitchWhenInstalled = false)]
                """;

            const string json = $$"""
                {
                    "Author": "",
                    "CodePath": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.exe",
                    "Description": "",
                    "Icon": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.png",
                    "Name": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}",
                    "OS": [
                        {
                            "MinimumVersion": "10",
                            "Platform": "windows"
                        }
                    ],
                    "Profiles": [
                        {
                            "DeviceType": 0,
                            "DontAutoSwitchWhenInstalled": false,
                            "Name": "Profile 1",
                            "Readonly": false
                        },
                        {
                            "DeviceType": 2,
                            "DontAutoSwitchWhenInstalled": true,
                            "Name": "Profile 2",
                            "Readonly": true
                        },
                        {
                            "DeviceType": 5,
                            "DontAutoSwitchWhenInstalled": false,
                            "Name": "Profile 3",
                            "Readonly": false
                        }
                    ],
                    "SDKVersion": 2,
                    "Software": {
                        "MinimumVersion": "5.0"
                    },
                    "Version": "0.0.0"
                }
                """;

            // Act, assert.
            Verify(sourceText, json);
        }

        [Test]
        public void WriteSingleAction()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action("My Action", "com.tests.example.myaction", "Images/Action", StateImage = "Images/State")]
                public class MyAction {}
                """;

            const string json = $$"""
                {
                    "Actions": [
                        {
                            "Icon": "Images/Action",
                            "Name": "My Action",
                            "States": [
                                {
                                    "Image": "Images/State"
                                }
                            ],
                            "UUID": "com.tests.example.myaction"
                        }
                    ],
                    "Author": "",
                    "CodePath": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.exe",
                    "Description": "",
                    "Icon": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.png",
                    "Name": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}",
                    "OS": [
                        {
                            "MinimumVersion": "10",
                            "Platform": "windows"
                        }
                    ],
                    "SDKVersion": 2,
                    "Software": {
                        "MinimumVersion": "5.0"
                    },
                    "Version": "0.0.0"
                }
                """;

            // Act, assert.
            Verify(sourceText, json);
        }

        /// <summary>
        /// Verifies the specified <paramref name="json"/> is generated from <see cref="ManifestSourceGenerator"/> when parsing <paramref name="sourceText"/> .
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="json">The expected JSON.</param>
        private static void Verify(string sourceText, string json)
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();

            // Act.
            SourceGeneratorTests.Run(
                new ManifestSourceGenerator(fileSystem.Object),
                sourceText);

            // Assert.
            fileSystem.Verify(f => f.WriteAllText(@"C:\temp\manifest.json", json, Encoding.UTF8), Times.Once);
        }
    }
}
