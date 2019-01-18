namespace SharpDeck.Events
{
    using Net;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// A factory that provides information about supported events received from an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckEventFactory
    {
        /// <summary>
        /// Initializes static members of the <see cref="StreamDeckClient"/> class.
        /// </summary>
        static StreamDeckEventFactory()
        {
            var typeOfClient = typeof(StreamDeckClient);
            var events = typeOfClient.GetEvents(BindingFlags.Instance | BindingFlags.Public);

            // construct the map, based on the structure of the stream deck client
            var dict = new Dictionary<string, StreamDeckEventInfo>();
            foreach (var ev in events)
            {
                var info = new StreamDeckEventInfo(ev, typeOfClient.GetField(ev.Name, BindingFlags.NonPublic | BindingFlags.Instance));
                if (!string.IsNullOrWhiteSpace(info.Name))
                {
                    dict.Add(info.Name, info);
                }
            }

            EVENTS_MAP = dict;
        }

        /// <summary>
        /// Gets the events map.
        /// </summary>
        private static IReadOnlyDictionary<string, StreamDeckEventInfo> EVENTS_MAP { get; }

        /// <summary>
        /// Tries to parse the request, when succesful, the event information and args are supplied.
        /// </summary>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs"/> instance containing the event data.</param>
        /// <param name="eventInfo">The event information.</param>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> when an event was parsed; otherwise <c>false</c>.</returns>
        public static bool TryParse(WebSocketMessageEventArgs e, out StreamDeckEventInfo eventInfo, out object args)
        {
            // attempt to determine which event was received
            var eventArgs = JsonConvert.DeserializeObject<StreamDeckEventArgs>(e.Message);
            if (EVENTS_MAP.TryGetValue(eventArgs.Event, out eventInfo))
            {
                args = JsonConvert.DeserializeObject(e.Message, eventInfo.ArgsType);
                return true;
            }

            args = null;
            return false;
        }
    }
}
