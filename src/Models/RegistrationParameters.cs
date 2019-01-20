namespace SharpDeck.Models
{
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Provides information about registration parameters supplied by an Elgato Stream Deck when initialising a client.
    /// </summary>
    public class RegistrationParameters
    {
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier string that should be used to register the plugin once the WebSocket is opened.
        /// </summary>
        public string PluginUUID { get; set; }

        /// <summary>
        /// Gets or sets the event type that should be used to register the plugin once the WebSocket is opened.
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Gets or sets the information about the Stream Deck application and devices information.
        /// </summary>
        public RegistrationInfo Info { get; set; }

        /// <summary>
        /// Attempts to parse the specified arguments to <see cref="RegistrationParameters"/>.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>The result of parsing.</returns>
        public static RegistrationParameters Parse(string[] args)
        {
            if (args.Length != 8)
            {
                throw new ArgumentException($"Invalid number of parameters: Expected 8, but was {args.Length}.");
            }

            var parameters = new RegistrationParameters();
            for (var i = 0; i < 4; i++)
            {
                var param = args[(i * 2)];
                var value = args[(i * 2) + 1];

                switch (param)
                {
                    case "-port":
                        parameters.Port = int.Parse(value);
                        break;
                    case "-pluginUUID":
                        parameters.PluginUUID = value;
                        break;
                    case "-registerEvent":
                        parameters.Event = value;
                        break;
                    case "-info":
                        parameters.Info = JsonConvert.DeserializeObject<RegistrationInfo>(value);
                        break;
                }
            }

            return parameters;
        }
    }
}
