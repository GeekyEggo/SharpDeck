namespace StreamDeck.Events
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides information about an action-based event received from an Elgato Stream Deck.
    /// </summary>
    public class ActionEventArgs<TPayload> : StreamDeckEventArgs<TPayload>
    {
        /// <summary>
        /// Gets the actions unique identifier. If your plugin supports multiple actions, you should use this value to see which action was triggered.
        /// </summary>
        [JsonInclude]
        public string? Action { get; internal set; }

        /// <summary>
        /// Gets an opaque value identifying the instances action. You will need to pass this opaque value to several APIs like the `setTitle` API.
        [JsonInclude]
        public string? Context { get; internal set; }

        /// <summary>
        /// Gets an opaque value identifying the device. Note that this opaque value will change each time you relaunch the Stream Deck application.
        [JsonInclude]
        public string? Device { get; internal set; }
    }
}
