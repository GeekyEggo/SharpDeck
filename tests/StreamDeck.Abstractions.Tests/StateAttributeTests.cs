namespace StreamDeck.Tests
{
    /// <summary>
    /// Provides assertions for <see cref="StateAttribute"/>.
    /// </summary>
    [TestFixture]
    public class StateAttributeTests
    {
        /// <summary>
        /// Asserts <see cref="StateAttribute"/> constructor initializes all properties
        /// </summary>
        [Test]
        public void Construct()
        {
            // Arrange, act.
            var state = new StateAttribute("Image.png")
            {
                FontFamily = FontFamily.ComicSansMS,
                FontSize = "13",
                FontStyle = FontStyle.Bold,
                FontUnderline = true,
                MultiActionImage = "MultiActionImage.png",
                Name = "State 1",
                ShowTitle = true,
                Title = "Title goes here",
                TitleAlignment = TitleAlignment.Bottom,
                TitleColor = "#FF0000"
            };

            // Assert.
            Assert.Multiple(() =>
            {
                Assert.That(state.FontFamily, Is.EqualTo(FontFamily.ComicSansMS));
                Assert.That(state.FontSize, Is.EqualTo("13"));
                Assert.That(state.FontStyle, Is.EqualTo(FontStyle.Bold));
                Assert.That(state.FontUnderline, Is.True);
                Assert.That(state.Image, Is.EqualTo("Image.png"));
                Assert.That(state.MultiActionImage, Is.EqualTo("MultiActionImage.png"));
                Assert.That(state.Name, Is.EqualTo("State 1"));
                Assert.That(state.ShowTitle, Is.True);
                Assert.That(state.Title, Is.EqualTo("Title goes here"));
                Assert.That(state.TitleAlignment, Is.EqualTo(TitleAlignment.Bottom));
                Assert.That(state.TitleColor, Is.EqualTo("#FF0000"));
            });
        }
    }
}
