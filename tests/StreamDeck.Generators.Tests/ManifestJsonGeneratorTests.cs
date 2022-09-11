namespace StreamDeck.Generators.Tests
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="ManifestJsonGenerator"/>, via the <see cref="PluginSourceGenerator"/>.
    /// </summary>
    [TestFixture]
    public class ManifestJsonGeneratorTests
    {
        [Test]
        public void ManifestReadsAssembly()
        {
            const string sourceText = """
                using StreamDeck;
                using System.Reflection;

                [assembly: AssemblyCompany("Bob Smith")]
                [assembly: AssemblyDescription("Hello world, this is a test")]
                [assembly: AssemblyVersion("12.34.56")]
                [assembly: Manifest(Icon = "Plugin.png")]]

                [ActionAttribute("Action One", "com.tests.example.one", "Action.png", StateImage = "State.png")]
                public class ActionOne
                {
                }
                """;

            const string json = $$"""
                {
                    "Actions": [
                        {
                            "Icon": "Action.png",
                            "Name": "Action One",
                            "States": [
                                {
                                    "Image": "State.png"
                                }
                            ],
                            "UUID": "com.tests.example.one"
                        }
                    ],
                    "Author": "Bob Smith",
                    "CodePath": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.exe",
                    "Description": "Hello world, this is a test",
                    "Icon": "Plugin.png",
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
            VerifySuccess(sourceText, json);
        }

        #region "TODO... again"

        /// <summary>
        /// Asserts <see cref="PluginSourceGenerator"/> generates a manifest file.
        /// </summary>
        [TestCase(TestName = "Create manifest from ManifestAttribute with full information")]
        public void CreateManifestFromAttribute()
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
            VerifySuccess(sourceText, json);
        }

        /// <summary>
        /// Asserts <see cref="PluginSourceGenerator"/> reads the assembly information, populating as much as possible.
        /// </summary>
        [TestCase(TestName = "Create manifest from ManifestAttribute with defaults and Assembly information")]
        public void CreateManifestWithAssemblyInfo()
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
            VerifySuccess(sourceText, json);
        }

        /// <summary>
        /// Asserts an error diagnostic is reported when the project directory cannot be determined.
        /// </summary>
        [TestCase(TestName = "Error when project directory is not found")]
        public void ErrorWhenProjectDirectoryNotFound()
        {
            // Arrange.
            const string sourceText = """
                [assembly: StreamDeck.Manifest]
                """;

            // Act, assert.
            VerifyFailure(
                sourceText,
                new MockAnalyzerConfigOptionsProvider(),
                (1, 1, "SD001", "Failed to generate manifest JSON file; unable to determine the project's directory from the compilation context.", DiagnosticSeverity.Error));
        }

        /// <summary>
        /// Asserts an error diagnostic is not reported when the project directory cannot be determined, as there is no <see cref="ManifestAttribute"/> present.
        /// </summary>
        [TestCase(TestName = "No error when project directory is not found, but ManifestAttribute not present")]
        public void NoErrorWhenProjectDirectoryNotFoundAndNoAttribute()
        {
            // Arrange.
            const string sourceText = """
                public class Foo
                {
                    public string Name { get; set; }
                }
                """;

            // Act, assert.
            VerifyFailure(
                sourceText,
                new MockAnalyzerConfigOptionsProvider());
        }

        /// <summary>
        /// Asserts <see cref="PluginSourceGenerator"/> adds profiles to the manifest.
        /// </summary>
        [TestCase(TestName = "Create manifest with profiles from ProfileAttribute")]
        public void CreateManifestWithProfiles()
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
            VerifySuccess(sourceText, json);
        }

        /// <summary>
        /// Asserts <see cref="PluginSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>.
        /// </summary>
        [TestCase(TestName = "Create manifest with action from ActionAttribute with defaults")]
        public void CreateWithActionAndDefaults()
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
            VerifySuccess(sourceText, json);
        }

        /// <summary>
        /// Asserts <see cref="PluginSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>.
        /// </summary>
        [TestCase(TestName = "Create manifest with action from ActionAttribute with full information")]
        public void CreateWithActionFuller()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action(
                    "My Action",
                    "com.tests.example.myaction",
                    "Images/Action",
                    StateImage = "Images/State",
                    PropertyInspectorPath = "pi/action.html",
                    SupportedInMultiActions = false,
                    Tooltip = "Hello world, this is a tool-tip",
                    VisibleInActionsList = false,
                    DisableCaching = true)]
                public class MyAction {}
                """;

            const string json = $$"""
                {
                    "Actions": [
                        {
                            "DisableCaching": true,
                            "Icon": "Images/Action",
                            "Name": "My Action",
                            "PropertyInspectorPath": "pi/action.html",
                            "States": [
                                {
                                    "Image": "Images/State"
                                }
                            ],
                            "SupportedInMultiActions": false,
                            "Tooltip": "Hello world, this is a tool-tip",
                            "UUID": "com.tests.example.myaction",
                            "VisibleInActionsList": false
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
            VerifySuccess(sourceText, json);
        }

        /// <summary>
        /// Asserts <see cref="PluginSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>.
        /// </summary>
        [TestCase(TestName = "Create manifest with multiple actions")]
        public void MultipleActions()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action("Action One", "com.tests.example.actionone", "Images/ActionOne", StateImage = "Images/StateOne")]
                public class ActionOne {}

                [Action("Action Two", "com.tests.example.actiontwo", "Images/ActionTwo", StateImage = "Images/StateTwo")]
                public class ActionTwo {}
                """;

            const string json = $$"""
                {
                    "Actions": [
                        {
                            "Icon": "Images/ActionOne",
                            "Name": "Action One",
                            "States": [
                                {
                                    "Image": "Images/StateOne"
                                }
                            ],
                            "UUID": "com.tests.example.actionone"
                        },
                        {
                            "Icon": "Images/ActionTwo",
                            "Name": "Action Two",
                            "States": [
                                {
                                    "Image": "Images/StateTwo"
                                }
                            ],
                            "UUID": "com.tests.example.actiontwo"
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
            VerifySuccess(sourceText, json);
        }

        /// <summary>
        /// Asserts the <see cref="ActionAttribute.UUID"/> is valid.
        /// </summary>
        [TestCase(TestName = "Error when action UUID contains invalid characters")]
        public void ErrorWhenInvalidUUID()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action("My Action", "Foo Bar", "Images/Action", StateImage = "Imgages/State")]
                public class MyAction {}
                """;

            // Act, assert.
            VerifyFailure(
                sourceText,
                (6, 14, "SD101", "Action 'My Action' must have a valid UUID; identifiers can only contain lowercase alphanumeric characters (a-z, 0-9), hyphens (-), and periods (.).", DiagnosticSeverity.Error));
        }

        /// <summary>
        /// Asserts the <see cref="ActionAttribute.StageImage"/> is defined.
        /// </summary>
        [TestCase(TestName = "Error when action does not define a StateImage")]
        public void ErrorWhenNoStateImage()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action("My Action", "com.tests.example.myaction", "Images/Action")]
                public class MyAction {}
                """;

            // Act, assert.
            VerifyFailure(
                sourceText,
                (6, 14, "SD102", "Action 'My Action' must have a state image; set the 'ActionAttribute.StateImage', or add a 'StateAttribute'.", DiagnosticSeverity.Error));
        }

        /// <summary>
        /// Asserts the <see cref="ActionAttribute.StateImage"/> and <see cref="StateAttribute"/> aren't both defined on the class.
        /// </summary>
        [TestCase(TestName = "Error when action contains StateImage and StateAttribute")]
        public void ErrorWhenStateImageAndStateAttribute()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action("My Action", "com.tests.example.myaction", "Images/Action", StateImage = "ImageFromAction")]
                [State("1")]
                public class MyAction {}
                """;

            // Act, assert.
            VerifyFailure(
                sourceText,
                (7, 14, "SD103", "Action 'My Action' must not set the 'ActionAttribute.StateImage' when a 'StateAttribute' is present.", DiagnosticSeverity.Error));
        }

        /// <summary>
        /// Asserts <see cref="StateAttribute"/> isn't defined more than twice.
        /// </summary>
        [TestCase(TestName = "Error when action has too many states")]
        public void ErrorWhenTooManyStates()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action("My Action", "com.tests.example.myaction", "Images/Action")]
                [State("1")]
                [State("2")]
                [State("3")]
                public class MyAction {}
                """;

            // Act, assert.
            VerifyFailure(
                sourceText,
                (9, 14, "SD104", "Action 'My Action' cannot have more than two states ('StateAttribute').", DiagnosticSeverity.Error));
        }

        /// <summary>
        /// Asserts <see cref="PluginSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>, and their <see cref="StateAttribute"/>.
        /// </summary>
        [TestCase(TestName = "Create manifest with action with single state")]
        public void State_Single()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action("My Action", "com.tests.example.myaction", "Images/Action")]
                [State(
                    "Images/State",
                    FontFamily = FontFamily.Arial,
                    FontSize = "13",
                    FontStyle = FontStyle.BoldItalic,
                    FontUnderline = true,
                    MultiActionImage = "Images/MultiAction",
                    Name = "State 0",
                    ShowTitle = false,
                    Title = "Hello world",
                    TitleColor = "#FF00FF",
                    TitleAlignment = TitleAlignment.Bottom)]
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
                                    "FontFamily": "Arial",
                                    "FontSize": "13",
                                    "FontStyle": "Bold Italic",
                                    "FontUnderline": true,
                                    "Image": "Images/State",
                                    "MultiActionImage": "Images/MultiAction",
                                    "Name": "State 0",
                                    "ShowTitle": false,
                                    "Title": "Hello world",
                                    "TitleAlignment": "bottom",
                                    "TitleColor": "#FF00FF"
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
            VerifySuccess(sourceText, json);
        }

        /// <summary>
        /// Asserts <see cref="PluginSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>, , and their <see cref="StateAttribute"/>.
        /// </summary>
        [TestCase(TestName = "Create manifest with action states with multiple states")]
        public void State_Multiple()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action("My Action", "com.tests.example.myaction", "Images/Action")]
                [State(
                    "Images/State0",
                    FontFamily = FontFamily.Arial,
                    FontSize = "1",
                    FontStyle = FontStyle.Regular,
                    FontUnderline = true,
                    MultiActionImage = "Images/MultiAction0",
                    Name = "State 0",
                    ShowTitle = false,
                    Title = "Hello world (0)",
                    TitleColor = "#FF0000",
                    TitleAlignment = TitleAlignment.Top)]
                [State(
                    "Images/State1",
                    FontFamily = FontFamily.Tahoma,
                    FontSize = "2",
                    FontStyle = FontStyle.Bold,
                    FontUnderline = true,
                    MultiActionImage = "Images/MultiAction1",
                    Name = "State 1",
                    ShowTitle = false,
                    Title = "Hello world (1)",
                    TitleColor = "#0000FF",
                    TitleAlignment = TitleAlignment.Bottom)]
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
                                    "FontFamily": "Arial",
                                    "FontSize": "1",
                                    "FontStyle": "Regular",
                                    "FontUnderline": true,
                                    "Image": "Images/State0",
                                    "MultiActionImage": "Images/MultiAction0",
                                    "Name": "State 0",
                                    "ShowTitle": false,
                                    "Title": "Hello world (0)",
                                    "TitleAlignment": "top",
                                    "TitleColor": "#FF0000"
                                },
                                {
                                    "FontFamily": "Tahoma",
                                    "FontSize": "2",
                                    "FontStyle": "Bold",
                                    "FontUnderline": true,
                                    "Image": "Images/State1",
                                    "MultiActionImage": "Images/MultiAction1",
                                    "Name": "State 1",
                                    "ShowTitle": false,
                                    "Title": "Hello world (1)",
                                    "TitleAlignment": "bottom",
                                    "TitleColor": "#0000FF"
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
            VerifySuccess(sourceText, json);
        }

        #endregion

        /// <summary>
        /// Verifies the specified <paramref name="expectedJson"/> is generated from <see cref="PluginSourceGenerator"/> when parsing <paramref name="sourceText"/> .
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="expectedJson">The expected JSON.</param>
        private static void VerifySuccess(string sourceText, string expectedJson)
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();

            // Act.
            var (_, resultDiagnostics) = SourceGeneratorTests.Run(
                new PluginSourceGenerator(fileSystem.Object),
                sourceText);

            // Assert.
            fileSystem.Verify(f => f.WriteAllText(@"C:\temp\manifest.json", expectedJson, Encoding.UTF8), Times.Once);
            Assert.That(resultDiagnostics, Is.Empty);
        }

        /// <summary>
        /// Verifies the specified <paramref name="expectedDiagnostics"/> are added to the context from <see cref="PluginSourceGenerator"/> when parsing <paramref name="sourceText"/> .
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="expectedDiagnostics">The expected collection of <see cref="Diagnostic"/>.</param>
        private static void VerifyFailure(string sourceText, params ExpectedDiagnostic[] expectedDiagnostics)
            => VerifyFailure(sourceText, SourceGeneratorTests.DEFAULT_OPTIONS_PROVIDER, expectedDiagnostics);

        /// <summary>
        /// Verifies the specified <paramref name="expectedDiagnostics"/> are added to the context from <see cref="PluginSourceGenerator"/> when parsing <paramref name="sourceText"/> .
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="optionsProvider">The <see cref="AnalyzerConfigOptionsProvider"/>.</param>
        /// <param name="expectedDiagnostics">The expected collection of <see cref="Diagnostic"/>.</param>
        private static void VerifyFailure(string sourceText, AnalyzerConfigOptionsProvider optionsProvider, params ExpectedDiagnostic[] expectedDiagnostics)
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();

            // Act.
            var (_, actualDiagnostics) = SourceGeneratorTests.Run(
                new PluginSourceGenerator(fileSystem.Object),
                sourceText,
                optionsProvider);

            // Assert.
            fileSystem.Verify(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Encoding>()), Times.Never);
            DiagnosticAssert.AreEqual(actualDiagnostics, expectedDiagnostics);
        }
    }
}
