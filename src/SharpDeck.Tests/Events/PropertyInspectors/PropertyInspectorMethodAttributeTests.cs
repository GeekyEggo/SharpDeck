namespace SharpDeck.Tests.Events.PropertyInspectors
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
        /// Asserts the <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> and <see cref="PropertyInspectorMethodAttribute.SendToPropertyInspectorEvent"/> are both set to <see cref="string.Empty"/> when neither are specified.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            // given, when
            var testCase = new PropertyInspectorMethodAttribute();

            // then
            Assert.That(testCase.SendToPluginEvent, Is.Empty);
            Assert.That(testCase.SendToPropertyInspectorEvent, Is.Empty);
        }

        /// <summary>
        /// Asserts the <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> and <see cref="PropertyInspectorMethodAttribute.SendToPropertyInspectorEvent"/> are both the same when only <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> is specified.
        /// </summary>
        /// <param name="sendToPluginEvent">The <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> constructor parameter value.</param>
        [TestCase(null)]
        [TestCase("toPlugin")]
        public void TestConstructor(string sendToPluginEvent)
        {
            // given, when
            var testCase = new PropertyInspectorMethodAttribute(sendToPluginEvent);

            // then
            Assert.That(testCase.SendToPluginEvent, Is.EqualTo(sendToPluginEvent));
            Assert.That(testCase.SendToPropertyInspectorEvent, Is.EqualTo(sendToPluginEvent));
        }

        /// <summary>
        /// Asserts the <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> and <see cref="PropertyInspectorMethodAttribute.SendToPropertyInspectorEvent"/> are both set independently when both are specified.
        /// </summary>
        /// <param name="sendToPluginEvent">The <see cref="PropertyInspectorMethodAttribute.SendToPluginEvent"/> constructor parameter value.</param>
        /// <param name="sendToPropertyInspectorEvent">The <see cref="PropertyInspectorMethodAttribute.SendToPropertyInspectorEvent"/> constructor parameter value.</param>
        [TestCase(null, null)]
        [TestCase("toPlugin", null)]
        [TestCase(null, "toPropertyInspector")]
        [TestCase("toPlugin", "toPropertyInspector")]
        [TestCase("same", "same")]
        public void TestConstructor(string sendToPluginEvent, string sendToPropertyInspectorEvent)
        {
            // given, when
            var testCase = new PropertyInspectorMethodAttribute(sendToPluginEvent, sendToPropertyInspectorEvent);

            // then
            Assert.That(testCase.SendToPluginEvent, Is.EqualTo(sendToPluginEvent));
            Assert.That(testCase.SendToPropertyInspectorEvent, Is.EqualTo(sendToPropertyInspectorEvent));
        }
    }
}
