namespace SharpDeck.Events.Sent.Feedback
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides feedback information relating to a volume layout.
    /// </summary>
    public class VolumeFeedback
    {
        /// <summary>
        /// Gets or sets the icon shown within the layout.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; set; } = null;

        /// <summary>
        /// Gets or sets the indicator.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public VolumeIndicator Indicator { get; set; } = null;

        /// <summary>
        /// Gets or sets the title of the action.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; } = null;

        /// <summary>
        /// Gets or sets the value to be displayed within the layout.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; } = null;
    }
}
