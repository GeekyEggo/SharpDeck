namespace StreamDeck
{
    using System.Text.Json;
    using StreamDeck.Events;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides information about registration parameters supplied by an Elgato Stream Deck when initialising a client.
    /// </summary>
    public class RegistrationParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationParameters"/> class.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <exception cref="ArgumentException">{option} was not defined in the command line arguments.</exception>
        public RegistrationParameters(string[] args)
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
                        this.Info = JsonSerializer.Deserialize(args[++i], StreamDeckJsonContext.Default.RegistrationInfo)!;
                        break;

                    default:
                        i--;
                        break;
                }
            }

            if (this.Event == null
                || this.Event == string.Empty)
            {
                throw new ArgumentException("-registerEvent was not defined in the command line arguments.", nameof(args));
            }

            if (this.Info == null)
            {
                throw new ArgumentException("-info was not defined in the command line arguments.", nameof(args));
            }

            if (this.Port == null)
            {
                throw new ArgumentException("-port was not defined in the command line arguments.", nameof(args));
            }

            if (this.PluginUUID == null
                || this.PluginUUID == string.Empty)
            {
                throw new ArgumentException("-pluginUUID was not defined in the command line arguments.", nameof(args));
            }
        }

        /// <summary>
        /// Gets the event type that should be used to register the plugin once the WebSocket is opened.
        /// </summary>
        public string Event { get; }

        /// <summary>
        /// Gets the information about the Stream Deck application and devices information.
        /// </summary>
        public RegistrationInfo Info { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int? Port { get; }

        /// <summary>
        /// Gets a unique identifier string that should be used to register the plugin once the WebSocket is opened.
        /// </summary>
        public string PluginUUID { get; }
    }
}
