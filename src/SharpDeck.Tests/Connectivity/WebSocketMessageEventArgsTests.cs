namespace SharpDeck.Tests.Connectivity
{
    using NUnit.Framework;
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides assertions for expected behaviour within <see cref="WebSocketMessageEventArgs"/>.
    /// </summary>
    [TestFixture]
    public class WebSocketMessageEventArgsTests
    {
        /// <summary>
        /// Tests the <see cref="WebSocketMessageEventArgs(string)"/> correct sets the properties of the instance.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Hello World")]
        public void TestConstructor(string message)
        {
            // given, when, then
            var testCase = new WebSocketMessageEventArgs(message);
            Assert.AreEqual(message, testCase.Message, "Message was not set by the constructor");
        }
    }
}
