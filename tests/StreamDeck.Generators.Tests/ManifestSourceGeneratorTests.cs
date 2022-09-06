namespace StreamDeck.Generators.Tests
{
    using System.Runtime.CompilerServices;
    using StreamDeck.Generators.IO;
    using StreamDeck.Generators.Tests.Helpers;

    [TestFixture]
    public class ManifestSourceGeneratorTests : GeneratorTestBase
    {
        [Test]
        public void DoSomething()
        {
            var fileSystem = new Mock<IFileSystem>();
            const string sourceText = """
            [assembly: StreamDeck.Manifest]

            namespace Foo
            {
                public class Bar
                {

                }
            }
            """;

            RunGenerator(sourceText, new ManifestSourceGenerator(fileSystem.Object), ("build_property.projectdir", @"C:\temp\"));
        }
    }
}
