namespace StreamDeck
{
    using System.Text.Json;
    using StreamDeck.Events;

    /// <summary>
    /// Provides information about registration parameters supplied by an Elgato Stream Deck when initialising a client.
    /// </summary>
    internal class RegistrationParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationParameters"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        internal RegistrationParameters(string[] args)
        {
            for (var i = 0; i < args.Length - 1; i++)
            {
                var param = args[i];
                var value = args[++i];

                switch (param)
                {
                    case "-port":
                        this.Port = int.Parse(value);
                        break;

                    case "-pluginUUID":
                        this.PluginUUID = value;
                        break;

                    case "-registerEvent":
                        this.Event = value;
                        break;

                    case "-info":
                        this.Info = JsonSerializer.Deserialize<RegistrationInfo>(value);
                        break;

                    default:
                        i--;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the event type that should be used to register the plugin once the WebSocket is opened.
        /// </summary>
        public string? Event { get; }

        /// <summary>
        /// Gets the information about the Stream Deck application and devices information.
        /// </summary>
        public RegistrationInfo? Info { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int? Port { get; }

        /// <summary>
        /// Gets a unique identifier string that should be used to register the plugin once the WebSocket is opened.
        /// </summary>
        public string? PluginUUID { get; }
    }
}
