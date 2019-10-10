namespace SharpDeck
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Provides internal constants used by the SharpDeck library.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Gets the default JSON settings.
        /// </summary>
        internal static JsonSerializerSettings DEFAULT_JSON_SETTINGS { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.None
        };
    }
}
