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
                new ExpectedDiagnostic(3, 12, "SDM01", "Manifest 'Description' not defined; consider setting 'ManifestAttribute.Description'", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(3, 12, "SDM02", "Manifest 'Icon' not defined; consider setting 'ManifestAttribute.Icon'", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(5, 2, "SDA01", "Action 'Icon' not defined; consider setting 'ActionAttribute.Icon'", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(5, 2, "SDA02", "Action 'StateImage' not defined; consider setting 'ActionAttribute.StateImage'", DiagnosticSeverity.Warning));
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
                new ExpectedDiagnostic(8, 2, "SDA01", "Action 'Icon' not defined; consider setting 'ActionAttribute.Icon'", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(8, 2, "SDA02", "Action 'StateImage' not defined; consider setting 'ActionAttribute.StateImage'", DiagnosticSeverity.Warning));
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
                new ExpectedDiagnostic(11, 5, "SDA04", "Action 'StateImage' can be removed as one or more 'StateAttribute' are present", DiagnosticSeverity.Warning));
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
                new ExpectedDiagnostic(12, 2, "SDA03", "Actions cannot have more than 2 states", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(13, 2, "SDA03", "Actions cannot have more than 2 states", DiagnosticSeverity.Warning));
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
        }

        /// <summary>
        /// Asserts <see cref="ManifestJsonGenerator.Generate(GeneratorExecutionContext, Analyzers.ManifestAnalyzer, IFileSystem)"/> errors when required profile fields are explicitly null.
        /// </summary>
        [TestCase(TestName = "Error when profile has explicitly null required fields")]
        public void Error_Profile_FieldsNull()
        {
            /*
             * Profile.Name
             * Profile.DeviceType?
             */
        }

        #region Old

        /*
        [Test]
        public void ManifestWarnsWithDefaults()
        {
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest]

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
                    "Author": "",
                    "CodePath": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.exe",
                    "Description": "",
                    "Icon": "",
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
            VerifySuccess(
                sourceText,
                json,
                new ExpectedDiagnostic(3, 12, "SD005", "The manifest.json file requires an 'Author'; consider specifying the 'Author', or adding 'AssemblyCompanyAttribute'.", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(3, 12, "SD006", "The manifest.json file requires a 'Description'; consider specifying the 'Description', or adding 'AssemblyDescriptionAttribute'.", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(3, 12, "SD003", "The manifest.json file requires the 'Icon' to be specified.", DiagnosticSeverity.Warning));
        }

        [Test]
        public void ManifestWarnsWithNull()
        {
            const string sourceText = """
                using StreamDeck;

                [assembly: Manifest(
                    Author = null,
                    CodePath = null,
                    Description = null,
                    Icon = null,
                    Name = null,
                    Version = null)]

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
                    "Author": "",
                    "CodePath": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.exe",
                    "Description": "",
                    "Icon": "",
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
            VerifySuccess(
                sourceText,
                json,
                new ExpectedDiagnostic(4, 5, "SD005", "The manifest.json file requires an 'Author'; consider specifying the 'Author', or adding 'AssemblyCompanyAttribute'.", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(5, 5, "SD002", "The 'CodePath' value was ignored when creating the manifest.json file; value should not be null or empty.", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(6, 5, "SD006", "The manifest.json file requires a 'Description'; consider specifying the 'Description', or adding 'AssemblyDescriptionAttribute'.", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(7, 5, "SD003", "The manifest.json file requires the 'Icon' to be specified.", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(8, 5, "SD002", "The 'Name' value was ignored when creating the manifest.json file; value should not be null or empty.", DiagnosticSeverity.Warning),
                new ExpectedDiagnostic(9, 5, "SD003", "The manifest.json file requires the 'Version' to be specified.", DiagnosticSeverity.Warning));
        }

        [Test]
        public void ManifestReadsAssembly()
        {
            const string sourceText = """
                using StreamDeck;
                using System.Reflection;

                [assembly: AssemblyProduct("Super Cool Plugin")]
                [assembly: AssemblyCompany("Bob Smith")]
                [assembly: AssemblyDescription("Hello world, Hello world")]
                [assembly: AssemblyVersion("12.34.56")]
                [assembly: Manifest(Icon = "Plugin.png")]

                [Action]
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
                    "Description": "Hello world, Hello world",
                    "Icon": "Plugin.png",
                    "Name": "Super Cool Plugin",
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
        */
        /*
        #region "TODO... again"


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
                [assembly: AssemblyDescription("Hello world, Hello world")]
                [assembly: AssemblyCompany("Bob Smith")]
                [assembly: AssemblyVersion("12.34.56")]
                """;

            const string json = $$"""
                {
                    "Author": "Bob Smith",
                    "CodePath": "{{SourceGeneratorTests.DEFAULT_ASSEMBLY_NAME}}.exe",
                    "Description": "Hello world, Hello world",
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

        */

        #endregion

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
            fileSystem.Verify(f => f.WriteAllText(@"C:\temp\manifest.json", expectedJson, Encoding.UTF8), Times.Once);
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
            fileSystem.Verify(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Encoding>()), Times.Never);
            DiagnosticAssert.AreEqual(actualDiagnostics, expectedDiagnostics);
        }
    }
}
