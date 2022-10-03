namespace StreamDeck.Tests.Helpers
{
    /// <summary>
    /// Provides helper methods for <see cref="RegistrationParameters"/>.
    /// </summary>
    internal static class RegistrationParametersHelper
    {
        /// <summary>
        /// Creates the a valid set of <see cref="RegistrationParameters"/>.
        /// </summary>
        /// <returns>The <see cref="RegistrationParameters"/>.</returns>
        public static RegistrationParameters CreateRegistrationParameters()
            => new RegistrationParameters("-port", "13", "-pluginUUID", "ABCDEF123456", "-registerEvent", "registerPlugin", "-info", "{}");
    }
}
