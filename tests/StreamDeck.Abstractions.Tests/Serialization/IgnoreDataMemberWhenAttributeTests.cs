namespace StreamDeck.Tests.Serialization
{
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides assertions for <see cref="IgnoreDataMemberWhenAttribute"/>.
    /// </summary>
    public class IgnoreDataMemberWhenAttributeTests
    {
        /// <summary>
        /// Asserts <see cref="IgnoreDataMemberWhenAttribute"/> constructor initializes and all properties.
        /// </summary>
        [Test]
        public void Construct()
        {
            // Arrange, act, assert.
            var attr = new IgnoreDataMemberWhenAttribute("Hello world");
            Assert.That(attr.Value, Is.EqualTo("Hello world"));
        }
    }
}
