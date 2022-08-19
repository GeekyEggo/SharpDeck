namespace StreamDeck.Payloads
{
    /// <summary>
    /// Provides URL payload information used to open a URL.
    /// </summary>
    public class UrlPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlPayload"/> class.
        /// </summary>
        /// <param name="url">The URL to open in the default browser.</param>
        public UrlPayload(string url)
            => this.Url = url;

        /// <summary>
        /// Gets the URL to open in the default browser.
        /// </summary>
        public string Url { get; }
    }
}
