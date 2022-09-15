namespace StreamDeck.Generators.Models
{
    /// <summary>
    /// Provides information about the minimum required version of Stream Deck.
    /// </summary>
    internal class Software
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Software"/> class.
        /// </summary>
        /// <param name="minimumVersion">The minimum version.</param>
        public Software(string minimumVersion)
            => this.MinimumVersion = minimumVersion;

        /// <summary>
        /// Gets or sets the value that indicates which version of the Stream Deck application is required to install the plugin.
        /// </summary>
        public string MinimumVersion { get; set; }
    }
}
