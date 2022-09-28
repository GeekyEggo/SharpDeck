namespace StreamDeck.Tests
{
    /// <summary>
    /// Provides assertions for <see cref="ActionAttribute"/>.
    /// </summary>
    [TestFixture]
    public class ActionAttributeTests
    {
        /// <summary>
        /// Asserts <see cref="ActionAttribute"/> constructor initializes all properties.
        /// </summary>
        [Test]
        public void Construct()
        {
            // Arrange, act.
            var action = new ActionAttribute
            {
                DisableCaching = true,
                Icon = "Icon.png",
                Name = "My Action",
                PropertyInspectorPath = "pi/index.html",
                StateImage = "StateImage.png",
                SupportedInMultiActions = true,
                Tooltip = "Tooltip...",
                UUID = "com.tests.abstractions.action",
                VisibleInActionsList = true
            };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(action.DisableCaching, Is.True);
                Assert.That(action.Icon, Is.EqualTo("Icon.png"));
                Assert.That(action.Name, Is.EqualTo("My Action"));
                Assert.That(action.PropertyInspectorPath, Is.EqualTo("pi/index.html"));
                Assert.That(action.StateImage, Is.EqualTo("StateImage.png"));
                Assert.That(action.States, Has.Count.EqualTo(1));
                Assert.That(action.States[0].Image, Is.EqualTo("StateImage.png"));
                Assert.That(action.SupportedInMultiActions, Is.True);
                Assert.That(action.Tooltip, Is.EqualTo("Tooltip..."));
                Assert.That(action.UUID, Is.EqualTo("com.tests.abstractions.action"));
                Assert.That(action.VisibleInActionsList, Is.True);
            });
        }
    }
}
