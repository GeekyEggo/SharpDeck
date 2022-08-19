namespace StreamDeck.Events.Sent
{
    /// <summary>
    /// Provides payload information used to switch the profile.
    /// </summary>
    public class SwitchToProfilePayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchToProfilePayload"/> class.
        /// </summary>
        /// <param name="profile">The name of the profile to switch to. The name should be identical to the name provided in the manifest.json file.</param>
        public SwitchToProfilePayload(string profile)
            => this.Profile = profile;

        /// <summary>
        /// Gets the name of the profile to switch to. The name should be identical to the name provided in the manifest.json file.
        /// </summary>
        public string Profile { get; }
    }
}
