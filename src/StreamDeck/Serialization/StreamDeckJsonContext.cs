namespace StreamDeck.Serialization
{
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization;
    using StreamDeck.Events;

    /// <summary>
    /// Provides a JSON context that contains information to assist with the serialization and deserialization of JSON objects.
    /// </summary>
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        WriteIndented = false)]

    // Global
    [JsonSerializable(typeof(StreamDeckEventArgs<ApplicationPayload>))]
    [JsonSerializable(typeof(DeviceConnectEventArgs))]
    [JsonSerializable(typeof(DeviceEventArgs))]
    [JsonSerializable(typeof(StreamDeckEventArgs))]
    [JsonSerializable(typeof(StreamDeckEventArgs<SettingsPayload>))]

    // Action Specific
    [JsonSerializable(typeof(ActionEventArgs<ActionPayload>))]
    [JsonSerializable(typeof(ActionEventArgs<KeyPayload>))]
    [JsonSerializable(typeof(ActionEventArgs<KeyPayload>))]
    [JsonSerializable(typeof(ActionEventArgs))]
    [JsonSerializable(typeof(ActionEventArgs))]
    [JsonSerializable(typeof(ActionEventArgs<JsonObject>))]
    [JsonSerializable(typeof(ActionEventArgs<TitlePayload>))]
    [JsonSerializable(typeof(ActionEventArgs<AppearancePayload>))]
    [JsonSerializable(typeof(ActionEventArgs<AppearancePayload>))]

    // Misc
    [JsonSerializable(typeof(RegistrationInfo))]
    internal partial class StreamDeckJsonContext : JsonSerializerContext
    {
    }
}
