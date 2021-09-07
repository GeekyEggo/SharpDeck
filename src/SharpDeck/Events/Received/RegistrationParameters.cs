namespace SharpDeck.Events.Received
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides information about registration parameters supplied by an Elgato Stream Deck when initialising a client.
    /// </summary>
    public class RegistrationParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationParameters"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <exception cref="ArgumentException">Invalid number of parameters: Expected 8, but was {args.Length}.</exception>
        public RegistrationParameters(string[] args)
        {
            args = args.Skip(1).Take(8).ToArray();
            if (args.Length != 8)
            {
                throw new ArgumentException($"Invalid number of parameters: Expected 8, but was {args.Length}.");
            }

            for (var i = 0; i < 4; i++)
            {
                var param = args[(i * 2)];
                var value = args[(i * 2) + 1];

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
                        this.Info = JsonConvert.DeserializeObject<RegistrationInfo>(value);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the event type that should be used to register the plugin once the WebSocket is opened.
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Gets or sets the information about the Stream Deck application and devices information.
        /// </summary>
        public RegistrationInfo Info { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier string that should be used to register the plugin once the WebSocket is opened.
        /// </summary>
        public string PluginUUID { get; set; }
    }
}
