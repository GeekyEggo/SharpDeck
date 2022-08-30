namespace StreamDeck.Tests.Serialization
{
    using System.Text.Json.Serialization;
    using StreamDeck.Tests.Helpers;

    /// <summary>
    /// Provides a JSON context that contains information to assist with the serialization and deserialization of JSON objects.
    /// </summary>
    [JsonSerializable(typeof(FooSettings))]
    public partial class TestJsonContext : JsonSerializerContext
    {
    }
}
