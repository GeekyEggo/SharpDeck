namespace SharpDeck.Events.Sent.Feedback
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides feedback information for a volume bar.
    /// </summary>
    public class VolumeIndicator
    {
        /// <summary>
        /// Gets or sets a value indicating whether the volume bar is enabled.
        /// </summary>
        [JsonProperty("enabled")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        public int Opacity { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public int Value { get; set; }
    }
}
