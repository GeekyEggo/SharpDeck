namespace SharpDeck.Events.Sent
{
    using Enums;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information about a payload that can be targetted.
    /// </summary>
    public class TargetPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetPayload"/> class.
        /// </summary>
        /// <param name="target">Specify if you want to display the title on the hardware and software, only on the hardware, or only on the software.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the target is set to all states.</param>
        public TargetPayload(TargetType target = TargetType.Both, int? state = null)
        {
            this.State = state;
            this.Target = target;
        }

        /// <summary>
        /// Gets or sets the state of the action to target.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? State { get; set; }

        /// <summary>
        /// Gets or sets a value indicating which display should be updated.
        /// </summary>
        public TargetType Target { get; set; }
    }
}
