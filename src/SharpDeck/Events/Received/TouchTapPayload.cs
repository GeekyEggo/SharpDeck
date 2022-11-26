namespace SharpDeck.Events.Received
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information about a touch-tap event.
    /// </summary>
    public class TouchTapPayload : SettingsPayload
    {

        /// <summary>
        /// Gets or sets the coordinates of a triggered action.
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Gets or sets the controller that triggered that action.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tap was held.
        /// </summary>
        public bool Hold { get; set; }

        /// <summary>
        /// Gets or sets the tap position.
        /// </summary>
        [JsonProperty("tapPos")]
        public int[] TapPosition { get; set; } = new int[0];
    }
}
