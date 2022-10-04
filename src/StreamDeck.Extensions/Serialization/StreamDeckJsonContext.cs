namespace StreamDeck.Extensions.Serialization
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using StreamDeck.Extensions.PropertyInspectors;

    /// <summary>
    /// Provides a JSON context that contains information to assist with the serialization and deserialization of JSON objects.
    /// </summary>
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
    [JsonSerializable(typeof(DataSourcePayload))]
    [JsonSerializable(typeof(DataSourceResponse))]

    [ExcludeFromCodeCoverage]
    internal partial class StreamDeckJsonContext : JsonSerializerContext
    {
    }
}
