namespace SharpDeck.Tests.Connectivity
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using SharpDeck.Connectivity;

    /// <summary>
    /// Provides assertions for expected behaviour within <see cref="StreamDeckActionCacheEntry"/>.
    /// </summary>
    [TestFixture]
    public class StreamDeckActionCacheEntryTests
    {
        /// <summary>
        /// Gets the test cases for <see cref="TestConstructor(string, StreamDeckAction)"/>.
        /// </summary>
        public static IEnumerable<TestCaseData> ConstructorTestCases
        {
            get
            {
                yield return new TestCaseData(null, null);
                yield return new TestCaseData(string.Empty, new StreamDeckAction());
                yield return new TestCaseData("Hello World", new StreamDeckAction());
            }
        }

        /// <summary>
        /// Tests the <see cref="StreamDeckActionCacheEntry(string, StreamDeckAction)"/> correct sets the properties of the instance.
        /// </summary>
        /// <param name="uuid">The UUID.</param>
        /// <param name="action">The action.</param>
        [TestCaseSource(nameof(ConstructorTestCases))]
        public void TestConstructor(string uuid, StreamDeckAction action)
        {
            // given
            var testCase = new StreamDeckActionCacheEntry(uuid, action);

            // when, then
            Assert.AreEqual(uuid, testCase.UUID, "UUID was not set by the constructor");
            Assert.AreEqual(action, testCase.Action, "Action was not set by the constructor");
        }
    }
}
