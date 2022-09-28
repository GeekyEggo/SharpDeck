namespace StreamDeck.Tests
{
    /// <summary>
    /// Provides assertions for <see cref="ProfileAttribute"/>.
    /// </summary>
    [TestFixture]
    public class ProfileAttributeTests
    {
        /// <summary>
        /// Asserts <see cref="ProfileAttribute"/> constructor initializes all properties
        /// </summary>
        [Test]
        public void Construct()
        {
            // Arrange, act.
            var profile = new ProfileAttribute("Profile Name", Device.StreamDeckXL)
            {
                DontAutoSwitchWhenInstalled = false,
                Readonly = true
            };

            // Assert.
            Assert.Multiple(() =>
            {
                Assert.That(profile.Name, Is.EqualTo("Profile Name"));
                Assert.That(profile.DeviceType, Is.EqualTo(Device.StreamDeckXL));
                Assert.That(profile.DontAutoSwitchWhenInstalled, Is.False);
                Assert.That(profile.Readonly, Is.True);
            });
        }
    }
}
