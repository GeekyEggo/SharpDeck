namespace SharpDeck.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Provides a standard test that should always pass.
    /// </summary>
    [TestFixture]
    public class TestCase
    {
        /// <summary>
        /// Tests that <c>true</c> equals <c>true</c>.
        /// </summary>
        [TestCase]
        public void Test()
            => Assert.True(true);
    }
}
