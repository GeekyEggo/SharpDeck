namespace SharpDeck.Events
{
    public class StreamDeckEventArgs<TPayload> : StreamDeckEventArgs
    {
        /// <summary>
        /// Gets or sets the main payload associated with the event.
        /// </summary>
        public TPayload Payload { get; set; }
    }
}
