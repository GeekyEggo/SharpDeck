namespace StreamDeck.Generators.Tests
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="ManifestJsonGenerator"/>.
    /// </summary>
    [TestFixture]
    public class ManifestJsonGeneratorTests
    {
        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> generates a manifest with only attributes and defaults.
        /// </summary>
        [TestCase(TestName = "Generate manifest with only attributes")]
        public void Generate_OnlyAttributes()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

                [Action]
                public class ActionOne {}
                """;

            var json = $$"""
                {
                    "Actions": [
                        {
                            "Icon": "",
                            "Name": "ActionOne",
                            "States": [
                                {
                                    "Image": ""
                                }
                            ],
                            "UUID": "com.testproject.testproject.actionone"
                        }
                    ],
                    "Author": "Test Project",
                    "CodePath": "Test Project.exe",
                    "Description": "",
                    "Icon": "",
                    "Name": "Test Project",
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
            VerifySuccess(
                sourceText,
                json,
                "Test Project",
                new ExpectedDiagnostic(3, 12, "SDM051", "Stream Deck manifest 'Description' is not defined; consider setting 'ManifestAttribute.Description'", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(3, 12, "SDM051", "Stream Deck manifest 'Icon' is not defined; consider setting 'ManifestAttribute.Icon'", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(5, 2, "SDM051", "Stream Deck action 'Icon' is not defined; consider setting 'ActionAttribute.Icon'", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(5, 2, "SDM051", "Stream Deck action 'StateImage' is not defined; consider setting 'ActionAttribute.StateImage'", DiagnosticSeverity.Warning));
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> generates a manifest with assembly attributes and defaults.
        /// </summary>
        [TestCase(TestName = "Generate manifest with information Assembly attributes")]
        public void Generate_ReadAssemblyInfo()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;
                using System.Reflection;

                [assembly: AssemblyCompany("Bob Smith")]
                [assembly: AssemblyDescription("Hello world")]
                [assembly: Manifest(Icon = "Plugin.png")]

                [Action]
                public class ActionOne {}
                """;

            var json = $$"""
                {
                    "Actions": [
                        {
                            "Icon": "",
                            "Name": "ActionOne",
                            "States": [
                                {
                                    "Image": ""
                                }
                            ],
                            "UUID": "com.bobsmith.testproject.actionone"
                        }
                    ],
                    "Author": "Bob Smith",
                    "CodePath": "Test Project.exe",
                    "Description": "Hello world",
                    "Icon": "Plugin.png",
                    "Name": "Test Project",
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
            VerifySuccess(
                sourceText,
                json,
                "Test Project",
                new ExpectedDiagnostic(8, 2, "SDM051", "Stream Deck action 'Icon' is not defined; consider setting 'ActionAttribute.Icon'", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(8, 2, "SDM051", "Stream Deck action 'StateImage' is not defined; consider setting 'ActionAttribute.StateImage'", DiagnosticSeverity.Warning));
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> generates a manifest with all information.
        /// </summary>
        [TestCase(TestName = "Generate manifest with all information")]
        public void Generate_EverythingDefined()
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
        /// Asserts <see cref="ManifestJsonGenerator.Execute(GeneratorExecutionContext, StreamDeckSyntaxReceiver, Analyzers.ManifestAnalyzer)"/> generates profiles.
        /// </summary>
        [TestCase(TestName = "Generate profiles")]
        public void Generate_Profiles()
        {
            // Arrange.
            const string sourceText = """
                using StreamDeck;
                using System.Reflection;

                [assembly: AssemblyCompany("Bob Smith")]
                [assembly: AssemblyDescription("Hello world")]
                [assembly: Manifest(Icon = "Plugin.png")]

                [assembly: Profile("Profile 1", Device.StreamDeck)]
                [assembly: Profile("Profile 2", Device.StreamDeckXL, Readonly = true, DontAutoSwitchWhenInstalled = true)]
                [assembly: Profile("Profile 3", Device.StreamDeckPedal, Readonly = false, DontAutoSwitchWhenInstalled = false)]
                """;

            var json = $$"""
                {
                    "Author": "Bob Smith",
                    "CodePath": "TestProject.exe",
                    "Description": "Hello world",
                    "Icon": "Plugin.png",
                    "Name": "TestProject",
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
            VerifySuccess(
                sourceText,
                json);
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Execute(GeneratorExecutionContext, StreamDeckSyntaxReceiver, Analyzers.ManifestAnalyzer)"/> generates actions in the correct order.
        /// </summary>
        [TestCase(TestName = "Generate actions, order by SortIndex and Name")]
        public void Generate_ActionsInOrder()
        {
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest(
                    Author = "Foo",
                    Name = "Test",
                    Icon = "Plugin.png",
                    Description = "Hello world")]

                [Action(Icon = "Icon.png", StateImage = "State.png", Name = "Z-Action")]
                public class ActionOne
                {
                    // Should be 5th, no SortIndex, and alphabetically last by name.
                }

                [Action(Icon = "Icon.png", StateImage = "State.png", SortIndex = 2)]
                public class ActionTwo
                {
                    // Should be 3rd, before actions without a SortIndex.
                }

                [Action(Icon = "Icon.png", StateImage = "State.png", SortIndex = 0)]
                public class ActionThree
                {
                    // Should be 1st.
                }

                [Action(Icon = "Icon.png", StateImage = "State.png", SortIndex = 1)]
                public class ActionFour
                {
                    // Should be 2nd, after ActionThree.
                }

                [Action(Icon = "Icon.png", StateImage = "State.png")]
                public class ActionFive
                {
                    // Should be 4th, no SortIndex, but alphabetically first by name.
                }

                """;

            var json = """
                {
                    "Actions": [
                        {
                            "Icon": "Icon.png",
                            "Name": "ActionThree",
                            "States": [
                                {
                                    "Image": "State.png"
                                }
                            ],
                            "UUID": "com.foo.test.actionthree"
                        },
                        {
                            "Icon": "Icon.png",
                            "Name": "ActionFour",
                            "States": [
                                {
                                    "Image": "State.png"
                                }
                            ],
                            "UUID": "com.foo.test.actionfour"
                        },
                        {
                            "Icon": "Icon.png",
                            "Name": "ActionTwo",
                            "States": [
                                {
                                    "Image": "State.png"
                                }
                            ],
                            "UUID": "com.foo.test.actiontwo"
                        },
                        {
                            "Icon": "Icon.png",
                            "Name": "ActionFive",
                            "States": [
                                {
                                    "Image": "State.png"
                                }
                            ],
                            "UUID": "com.foo.test.actionfive"
                        },
                        {
                            "Icon": "Icon.png",
                            "Name": "Z-Action",
                            "States": [
                                {
                                    "Image": "State.png"
                                }
                            ],
                            "UUID": "com.foo.test.z-action"
                        }
                    ],
                    "Author": "Foo",
                    "CodePath": "Test Project.exe",
                    "Description": "Hello world",
                    "Icon": "Plugin.png",
                    "Name": "Test",
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

            VerifySuccess(sourceText, json, "Test Project");
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Execute(GeneratorExecutionContext, StreamDeckSyntaxReceiver, Analyzers.ManifestAnalyzer)"/> generates actions with a property inspector path
        /// if their property inspector will be automatically generated.
        /// </summary>
        [TestCase(TestName = "Generate action, set property inspector path")]
        public void Generate_ActionPropertyInspectorPath()
        {
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest(
                    Author = "User",
                    Name = "Product",
                    Icon = "Plugin.png",
                    Description = "Hello world")]

                [Action(
                    Icon = "Icon.png",
                    StateImage = "State.png",
                    UUID = "com.user.product.action",
                    PropertyInspectorType = typeof(Settings))]
                public class ActionOne {}

                public class Settings {}

                """;

            var json = """
                {
                    "Actions": [
                        {
                            "Icon": "Icon.png",
                            "Name": "ActionOne",
                            "PropertyInspectorPath": "pi/actionone.g.html",
                            "States": [
                                {
                                    "Image": "State.png"
                                }
                            ],
                            "UUID": "com.user.product.action"
                        }
                    ],
                    "Author": "User",
                    "CodePath": "Test Project.exe",
                    "Description": "Hello world",
                    "Icon": "Plugin.png",
                    "Name": "Product",
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

            VerifySuccess(sourceText, json, "Test Project");
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> warns when the StateImage and States are both defined.
        /// </summary>
        [TestCase(TestName = "Warn when action has StateImage and States defined")]
        public void Warn_Action_HasStateImageAndStates()
        {
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest(
                    Author = "Foo",
                    Name = "Test",
                    Icon = "Plugin.png",
                    Description = "Hello world")]

                [Action(
                    Icon = "Action.png",
                    StateImage = "Foo.png")]
                [State("State1.png")]
                [State("State2.png")]
                public class ActionOne
                {
                }

                """;

            var json = """
                {
                    "Actions": [
                        {
                            "Icon": "Action.png",
                            "Name": "ActionOne",
                            "States": [
                                {
                                    "Image": "State1.png"
                                },
                                {
                                    "Image": "State2.png"
                                }
                            ],
                            "UUID": "com.foo.test.actionone"
                        }
                    ],
                    "Author": "Foo",
                    "CodePath": "Test Project.exe",
                    "Description": "Hello world",
                    "Icon": "Plugin.png",
                    "Name": "Test",
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

            VerifySuccess(
                sourceText,
                json,
                "Test Project",
                new ExpectedDiagnostic(11, 5, "SDM052", "Stream Deck action 'StateImage' can be removed as one or more 'StateAttribute' are present", DiagnosticSeverity.Warning));
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> warns when more than two states are defined.
        /// </summary>
        [TestCase(TestName = "Warn when action has too many states")]
        public void Warn_Action_TooManyStates()
        {
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest(
                    Author = "Foo",
                    Name = "Test",
                    Icon = "Plugin.png",
                    Description = "Hello world")]

                [Action(Icon = "Action.png")]
                [State("State1.png")]
                [State("State2.png")]
                [State("State3.png")]
                [State("State4.png")]
                public class ActionOne
                {
                }

                """;

            var json = """
                {
                    "Actions": [
                        {
                            "Icon": "Action.png",
                            "Name": "ActionOne",
                            "States": [
                                {
                                    "Image": "State1.png"
                                },
                                {
                                    "Image": "State2.png"
                                }
                            ],
                            "UUID": "com.foo.test.actionone"
                        }
                    ],
                    "Author": "Foo",
                    "CodePath": "Test Project.exe",
                    "Description": "Hello world",
                    "Icon": "Plugin.png",
                    "Name": "Test",
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

            VerifySuccess(
                sourceText,
                json,
                "Test Project",
                new ExpectedDiagnostic(12, 2, "SDM053", "Stream Deck actions cannot have more than 2 states", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(13, 2, "SDM053", "Stream Deck actions cannot have more than 2 states", DiagnosticSeverity.Warning));
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> errors when required manifest fields are explicitly null.
        /// </summary>
        [TestCase(TestName = "Error when manifest has explicitly null required fields")]
        public void Error_Manifest_FieldsNull()
        {
            /*
             * Manifest.Author
             * Manifest.CodePath
             * Manifest.Description
             * Manifest.Icon
             * Manifest.Name
             * Manifest.Version
             * Manifest.SDKVersion
             * Manifest.Software
             */
            Assert.Inconclusive();
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> errors when required action fields are explicitly null.
        /// </summary>
        [TestCase(TestName = "Error when action has explicitly null required fields")]
        public void Error_Action_FieldsNull()
        {
            /*
             * Action.Icon
             * Action.Name
             * Action.StatesImage
             * Action.UUID
             */
            Assert.Inconclusive();
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> errors when required state fields are explicitly null.
        /// </summary>
        [TestCase(TestName = "Error when state has explicitly null required fields")]
        public void Error_State_FieldsNull()
        {
            /*
             * Action.Icon
             * Action.Name
             * Action.StatesImage
             * Action.UUID
             */
            Assert.Inconclusive();
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> errors when required profile fields are explicitly null.
        /// </summary>
        [TestCase(TestName = "Error when profile has explicitly null required fields")]
        public void Error_Profile_FieldsNull()
        {
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest(Author = "Author", Name = "Name", Icon = "Icon.png", Description = "Description")]
                [assembly: Profile(null, Device.StreamDeck)]
                """;

            VerifyFailure(
                sourceText,
                new ExpectedDiagnostic(4, 20, "SDM001", "Stream Deck profile 'Name' cannot be null", DiagnosticSeverity.Error));
        }

        /// <summary>
        /// Verifies the specified <paramref name="expectedJson"/> is generated from <see cref="PluginSourceGenerator"/> when parsing <paramref name="sourceText"/> .
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="expectedJson">The expected JSON.</param>
        /// <param name="assemblyName">The optional assembly name.</param>
        /// <param name="expectedDiagnostics">The expected collection of <see cref="Diagnostic"/>.</param>
        private static void VerifySuccess(string sourceText, string expectedJson, string? assemblyName = null, params ExpectedDiagnostic[] expectedDiagnostics)
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();

            // Act.
            var (_, actualDiagnostics) = SourceGeneratorTests.Run(
                new ManifestJsonGenerator(fileSystem.Object),
                sourceText,
                assemblyName);

            // Assert.
            SourceGeneratorTests.VerifyFiles(fileSystem, ("manifest.json", expectedJson));
            DiagnosticAssert.AreEqual(actualDiagnostics, expectedDiagnostics);
        }

        /// <summary>
        /// Verifies the specified <paramref name="expectedDiagnostics"/> are added to the context from <see cref="ManifestJsonGenerator"/> when parsing <paramref name="sourceText"/> .
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="expectedDiagnostics">The expected collection of <see cref="Diagnostic"/>.</param>
        private static void VerifyFailure(string sourceText, params ExpectedDiagnostic[] expectedDiagnostics)
            => VerifyFailure(sourceText, SourceGeneratorTests.DEFAULT_OPTIONS_PROVIDER, expectedDiagnostics);

        /// <summary>
        /// Verifies the specified <paramref name="expectedDiagnostics"/> are added to the context from <see cref="ManifestJsonGenerator"/> when parsing <paramref name="sourceText"/> .
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
                new ManifestJsonGenerator(fileSystem.Object),
                sourceText,
                optionsProvider: optionsProvider);

            // Assert.
            SourceGeneratorTests.VerifyFiles(fileSystem);
            DiagnosticAssert.AreEqual(actualDiagnostics, expectedDiagnostics);
        }
    }
}
