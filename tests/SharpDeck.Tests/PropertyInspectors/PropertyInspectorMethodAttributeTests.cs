namespace SharpDeck.Tests.PropertyInspectors
{
    using NUnit.Framework;
    using SharpDeck.PropertyInspectors;

    /// <summary>
    /// Provides assertions for expected behaviour within <see cref="PropertyInspectorMethodAttribute"/>.
    /// </summary>
    [TestFixture]
    public class PropertyInspectorMethodAttributeTests
    {
        /// <summary>
        /// Asserts <see cref="PropertyInspectorMethodAttribute(string)"/> correctly sets the <see cref="PropertyInspectorMethodAttribute.EventName"/>.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>The actual <see cref="PropertyInspectorMethodAttribute.EventName"/>.</returns>
        [Test]
        [TestCase(null, ExpectedResult = null)]
        [TestCase("", ExpectedResult = "")]
        [TestCase("foo", ExpectedResult = "foo")]
        [TestCase("bar", ExpectedResult = "bar")]
        public string Constructor_SetsEventName(string eventName)
            => new PropertyInspectorMethodAttribute(eventName).EventName;
    }
}
