namespace SharpDeck.Events.Sent
{
    /// <summary>
    /// Provides payload information used to set the state.
    /// </summary>
    public class SetStatePayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetStatePayload"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public SetStatePayload(int state = 0)
        {
            this.State = state;
        }

        /// <summary>
        /// Gets or sets the 0-based integer value representing the state requested.
        /// </summary>
        public int State { get; set; }
    }
}
