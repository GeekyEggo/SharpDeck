namespace StreamDeck.Tests
{
    /// <summary>
    /// Provides assertions for <see cref="ManifestAttribute"/>.
    /// </summary>
    [TestFixture]
    public class ManifestAttributeTests
    {
        /// <summary>
        /// Asserts <see cref="ManifestAttribute"/> constructor initializes and all properties.
        /// </summary>
        [Test]
        public void Construct()
        {
            // Arrange, act.
            var manifest = new ManifestAttribute
            {
                ApplicationsToMonitorMac = new[] { "com.apple.terminal" },
                ApplicationsToMonitorWin = new[] { "notepad.exe" },
                Author = "Bob",
                Category = "Tests",
                CategoryIcon = "Tests.png",
                CodePath = "default.exe",
                CodePathMac = "com.tests.main",
                CodePathWin = "other.exe",
                DefaultWindowSize = new[] { 100, 200 },
                Description = "Hello world...",
                Icon = "Icon.png",
                Name = "My Manifest",
                OSMacMinimumVersion = "1",
                PropertyInspectorPath = "pi/default.html",
                Url = "https://example.com",
                Version = "13.0.1"
            };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(manifest.ApplicationsToMonitorMac, Has.Length.EqualTo(1));
                Assert.That(manifest.ApplicationsToMonitorMac[0], Is.EqualTo("com.apple.terminal"));
                Assert.That(manifest.ApplicationsToMonitorWin, Has.Length.EqualTo(1));
                Assert.That(manifest.ApplicationsToMonitorWin[0], Is.EqualTo("notepad.exe"));

                Assert.That(manifest.Author, Is.EqualTo("Bob"));
                Assert.That(manifest.Category, Is.EqualTo("Tests"));
                Assert.That(manifest.CategoryIcon, Is.EqualTo("Tests.png"));
                Assert.That(manifest.CodePath, Is.EqualTo("default.exe"));
                Assert.That(manifest.CodePathMac, Is.EqualTo("com.tests.main"));
                Assert.That(manifest.CodePathWin, Is.EqualTo("other.exe"));

                Assert.That(manifest.DefaultWindowSize, Has.Length.EqualTo(2));
                Assert.That(manifest.DefaultWindowSize[0], Is.EqualTo(100));
                Assert.That(manifest.DefaultWindowSize[1], Is.EqualTo(200));

                Assert.That(manifest.Description, Is.EqualTo("Hello world..."));
                Assert.That(manifest.Icon, Is.EqualTo("Icon.png"));
                Assert.That(manifest.Name, Is.EqualTo("My Manifest"));
                Assert.That(manifest.OSMacMinimumVersion, Is.EqualTo("1"));
                Assert.That(manifest.OSWindowsMinimumVersion, Is.EqualTo(ManifestAttribute.DEFAULT_OS_WINDOWS_MINIMUM_VERSION));
                Assert.That(manifest.PropertyInspectorPath, Is.EqualTo("pi/default.html"));
                Assert.That(manifest.SDKVersion, Is.EqualTo(ManifestAttribute.DEFAULT_SDK_VERSION));
                Assert.That(manifest.SoftwareMinimumVersion, Is.EqualTo(ManifestAttribute.DEFAULT_SOFTWARE_MINIMUM_VERSION));
                Assert.That(manifest.Url, Is.EqualTo("https://example.com"));
                Assert.That(manifest.Version, Is.EqualTo("13.0.1"));
            });
        }
    }
}
