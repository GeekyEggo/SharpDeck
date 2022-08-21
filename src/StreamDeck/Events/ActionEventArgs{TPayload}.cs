namespace StreamDeck.Events
{
    /// <summary>
    /// Provides information about an action-based event received from an Elgato Stream Deck.
    /// </summary>
    public class ActionEventArgs<TPayload> : PartialActionEventArgs<TPayload>
    {
        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        [JsonInclude]
        public string? Device { get; internal set; }
    }
}
