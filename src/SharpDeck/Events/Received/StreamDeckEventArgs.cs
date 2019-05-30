namespace SharpDeck.Events.Received
{
    using System;

    /// <summary>
    /// Provides information about an event received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        public string Event { get; set; }
    }
}
