namespace StreamDeck.Generators.Tests
{
    using System.Linq;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Tests.Helpers;

    /// <summary>
    /// Provides assertions for <see cref="UuidPropertySourceGenerator"/>, via the <see cref="PluginSourceGenerator"/>.
    /// </summary>
    [TestFixture]
    public class UuidPropertySourceGeneratorTests
    {
        /// <summary>
        /// Asserts no additional syntax trees are added when there are no actions.
        /// </summary>
        [TestCase(TestName = "No actions")]
        public void NoActions()
            => Verify(string.Empty);

        /// <summary>
        /// Asserts that non-partial classes are ignored.
        /// </summary>
        [TestCase(TestName = "Non partial classes")]
        public void NonPartialClasses()
            => Verify("""
                [StreamDeck.Action("My Action", "com.tests.example.myaction", "Images/Action")]
                public class MyAction {}
                """);

        /// <summary>
        /// Asserts that a class with a property of "UUID" is ignored.
        /// </summary>
        [TestCase(TestName = "Property name already exists")]
        public void PropertyExists()
            => Verify("""
                [StreamDeck.Action("My Action", "com.tests.example.myaction", "Images/Action")]
                public partial class MyAction
                {
                    public const string UUID = "foo";
                }
                """);

        /// <summary>
        /// Asserts that a class with a method of "UUID" is ignored.
        /// </summary>
        [TestCase(TestName = "Method name already exists")]
        public void MethodExists()
            => Verify("""
                [StreamDeck.Action("My Action", "com.tests.example.myaction", "Images/Action")]
                public partial class MyAction
                {
                    public void UUID()
                    {
                    }
                }
                """);

        /// <summary>
        /// Asserts the UUID property is written when the class is on the global namespace.
        /// </summary>
        [TestCase(TestName = "Single action on the global namespace")]
        public void GlobalScopeAction()
        {
            // Arrange.
            var sourceText = """
                [StreamDeck.Action("My Action", "com.tests.example.myaction", "Images/Action")]
                public partial class MyAction {}
                """;

            // Act, assert.
            Verify(
                sourceText,
                (
                    HintName: "com.tests.example.myaction.0.g.cs",
                    SourceText: """
                    // <auto-generated />

                    partial class MyAction
                    {
                        /// <summary>
                        /// Gets the unique identifier of the action as defined by the <see cref="StreamDeck.ActionAttribute.UUID"/>.
                        /// </summary>
                        public const string UUID = "com.tests.example.myaction";
                    }

                    """
                ));
        }

        /// <summary>
        /// Asserts the UUID property is written when the class is on a custom namespace.
        /// </summary>
        [TestCase(TestName = "Single action on a defined namespace")]
        public void NamespaceScopeAction()
        {
            // Arrange.
            var sourceText = """
                namespace Foo.Bar
                {
                    [StreamDeck.Action("My Action", "com.tests.example.myaction", "Images/Action")]
                    public partial class MyAction {}
                }
                """;

            // Act, assert.
            Verify(
                sourceText,
                (
                    HintName: "com.tests.example.myaction.0.g.cs",
                    SourceText: """
                    // <auto-generated />

                    namespace Foo.Bar
                    {
                        partial class MyAction
                        {
                            /// <summary>
                            /// Gets the unique identifier of the action as defined by the <see cref="StreamDeck.ActionAttribute.UUID"/>.
                            /// </summary>
                            public const string UUID = "com.tests.example.myaction";
                        }
                    }

                    """
                ));
        }

        /// <summary>
        /// Asserts the UUID property is written when the multiple classes are defined, on the global namespace and a custom namespace.
        /// </summary>
        [TestCase(TestName = "Two actions, one on either namespace scope")]
        public void GlobalScopeAndNamespaceScopeAction()
        {
            // Arrange.
            var sourceText = """
                [StreamDeck.Action("My Action", "com.tests.example.myaction", "Images/Action")]
                public partial class MyAction {}

                namespace Foo.Bar
                {
                    [StreamDeck.Action("My Other Action", "com.tests.example.myotheraction", "Images/Action")]
                    public partial class MyOtherAction {}
                }
                """;

            // Act, assert.
            Verify(
                sourceText,
                (
                    HintName: "com.tests.example.myaction.0.g.cs",
                    SourceText: """
                    // <auto-generated />

                    partial class MyAction
                    {
                        /// <summary>
                        /// Gets the unique identifier of the action as defined by the <see cref="StreamDeck.ActionAttribute.UUID"/>.
                        /// </summary>
                        public const string UUID = "com.tests.example.myaction";
                    }

                    """
                ),
                (
                    HintName: "com.tests.example.myotheraction.0.g.cs",
                    SourceText: """
                    // <auto-generated />

                    namespace Foo.Bar
                    {
                        partial class MyOtherAction
                        {
                            /// <summary>
                            /// Gets the unique identifier of the action as defined by the <see cref="StreamDeck.ActionAttribute.UUID"/>.
                            /// </summary>
                            public const string UUID = "com.tests.example.myotheraction";
                        }
                    }

                    """
                ));
        }

        /// <summary>
        /// Asserts the UUID property is written when the classes have a UUID conflict.
        /// </summary>
        [TestCase(TestName = "Multiple actions with duplicate UUID")]
        public void MultipleActionsWithDuplicateUUID()
        {
            // Arrange.
            var sourceText = """
                [StreamDeck.Action("My Action", "com.tests.example.myaction", "Images/Action")]
                public partial class MyAction {}

                [StreamDeck.Action("My Action", "com.tests.example.myaction", "Images/Action")]
                public partial class MyAction {}
                """;

            // Act, assert.
            Verify(
                sourceText,
                (
                    HintName: "com.tests.example.myaction.0.g.cs",
                    SourceText: """
                    // <auto-generated />

                    partial class MyAction
                    {
                        /// <summary>
                        /// Gets the unique identifier of the action as defined by the <see cref="StreamDeck.ActionAttribute.UUID"/>.
                        /// </summary>
                        public const string UUID = "com.tests.example.myaction";
                    }

                    """
                ),
                (
                    HintName: "com.tests.example.myaction.1.g.cs",
                    SourceText: """
                    // <auto-generated />

                    partial class MyAction
                    {
                        /// <summary>
                        /// Gets the unique identifier of the action as defined by the <see cref="StreamDeck.ActionAttribute.UUID"/>.
                        /// </summary>
                        public const string UUID = "com.tests.example.myaction";
                    }

                    """
                ));
        }

        /// <summary>
        /// Runs the specified <paramref name="sourceText"/>, and asserts the specified <paramref name="expectedSyntaxTrees"/> are on the output compilation.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="expectedSyntaxTrees">The expected syntax trees.</param>
        private static void Verify(string sourceText, params (string HintName, string SourceText)[] expectedSyntaxTrees)
        {
            // Arrange.
            var fileSystem = new Mock<IFileSystem>();

            // Act.
            var (outputCompilation, _) = SourceGeneratorTests.Run(
                new PluginSourceGenerator(fileSystem.Object),
                sourceText);

            // Assert.
            Assert.That(outputCompilation, Is.Not.Null);
            Assert.That(outputCompilation.SyntaxTrees.Count(), Is.EqualTo(expectedSyntaxTrees.Length + 2)); // Add 2; the source text syntax tree, and the HostExtensions syntax tree.

            var actualSyntaxTrees = outputCompilation
                .SyntaxTrees.Skip(1) // The source text syntax tree.
                .Take(expectedSyntaxTrees.Length)
                .ToArray();

            for (var i = 0; i < expectedSyntaxTrees.Length; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(actualSyntaxTrees[i].FilePath, Is.EqualTo($@"StreamDeck.Generators\StreamDeck.Generators.PluginSourceGenerator\{expectedSyntaxTrees[i].HintName}"));
                    Assert.That(actualSyntaxTrees[i].ToString(), Is.EqualTo(expectedSyntaxTrees[i].SourceText));
                });
            }
        }
    }
}
