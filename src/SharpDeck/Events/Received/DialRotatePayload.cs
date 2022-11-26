namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Provides information about a dial rotation.
    /// </summary>
    public class DialRotatePayload : DialPayload
    {
        /// <summary>
        /// Gets or sets the ticks. This indicates the direction of the rotation; negative is counter-clockwise, positive is clockwise
        /// </summary>
        public int Ticks { get; set; }
    }
}
