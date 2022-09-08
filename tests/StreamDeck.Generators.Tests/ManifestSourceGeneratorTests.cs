namespace StreamDeck.Generators.Tests
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="ManifestSourceGenerator"/>.
    /// </summary>
    [TestFixture]
    public class ManifestSourceGeneratorTests
    {
        #region Manifest

        /// <summary>
        /// Asserts <see cref="ManifestSourceGenerator"/> generates a manifest file.
        /// </summary>
        [Test]
        public void Manifest_FullInfo()
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
        /// Asserts <see cref="ManifestSourceGenerator"/> reads the assembly information, populating as much as possible.
        /// </summary>
        [Test]
        public void Manifest_ReadsAssemblyInfo()
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

        #endregion

        #region Profiles

        /// <summary>
        /// Asserts <see cref="ManifestSourceGenerator"/> adds profiles to the manifest.
        /// </summary>
        [Test]
        public void Profiles()
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

        #endregion

        #region Actions

        /// <summary>
        /// Asserts <see cref="ManifestSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>.
        /// </summary>
        [Test]
        public void Action_BasicInfo()
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
        /// Asserts <see cref="ManifestSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>.
        /// </summary>
        [Test]
        public void Action_FullInfo()
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
        /// Asserts <see cref="ManifestSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>.
        /// </summary>
        [Test]
        public void Action_Multiple()
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

        [Test]
        public void Action_FailsWhenNoStateImage()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action("My Action", "com.tests.example.myaction", "Images/Action", StateImage = "ImageFromAction")]
                [State("ImageFromState")]
                public class MyAction {}
                """;

            // Act, assert.
            VerifyFailure(
                sourceText,
                (7, 14, "SD101", "Action, 'My Action', should not set the 'ActionAttribute.StateImage' when a 'StateAttribute' is present.", DiagnosticSeverity.Error));
        }

        #endregion

        #region States

        /// <summary>
        /// Asserts <see cref="ManifestSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>, and their <see cref="StateAttribute"/>.
        /// </summary>
        [Test]
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
        /// Asserts <see cref="ManifestSourceGenerator"/> writes classes with <see cref="ActionAttribute"/>, , and their <see cref="StateAttribute"/>.
        /// </summary>
        [Test]
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
        /// Verifies the specified <paramref name="expectedJson"/> is generated from <see cref="ManifestSourceGenerator"/> when parsing <paramref name="sourceText"/> .
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="expectedJson">The expected JSON.</param>
        private static void VerifySuccess(string sourceText, string expectedJson)
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();

            // Act.
            var resultDiagnostics = SourceGeneratorTests.Run(
                new ManifestSourceGenerator(fileSystem.Object),
                sourceText);

            // Assert.
            fileSystem.Verify(f => f.WriteAllText(@"C:\temp\manifest.json", expectedJson, Encoding.UTF8), Times.Once);
            Assert.That(resultDiagnostics, Is.Empty);
        }

        /// <summary>
        /// Verifies the specified <paramref name="expectedDiagnostics"/> are added to the context from <see cref="ManifestSourceGenerator"/> when parsing <paramref name="sourceText"/> .
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="expectedDiagnostics">The expected collection of <see cref="Diagnostic"/>.</param>
        private static void VerifyFailure(string sourceText, params (int Row, int Column, string Id, string Description, DiagnosticSeverity Severity)[] expectedDiagnostics)
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();

            // Act.
            var actualDiagnostics = SourceGeneratorTests.Run(
                new ManifestSourceGenerator(fileSystem.Object),
                sourceText);

            // Assert.
            fileSystem.Verify(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Encoding>()), Times.Never);

            Assert.That(actualDiagnostics.Count, Is.EqualTo(expectedDiagnostics.Length));
            for (var i = 0; i < expectedDiagnostics.Length; i++)
            {
                var (Row, Column, Id, Description, Severity) = expectedDiagnostics[i];
                Assert.That(actualDiagnostics[i].ToString(), Is.EqualTo($"({Row},{Column}): {Severity.ToString().ToLower()} {Id}: {Description}"));
            }
        }
    }
}
