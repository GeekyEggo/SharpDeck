namespace StreamDeck.Serialization
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization;
    using StreamDeck.Events;
    using StreamDeck.Payloads;

    /// <summary>
    /// Provides a JSON context that contains information to assist with the serialization and deserialization of JSON objects.
    /// </summary>
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        WriteIndented = false)]

    // Receiving - Global.
    [JsonSerializable(typeof(StreamDeckEventArgs<ApplicationPayload>))]
    [JsonSerializable(typeof(DeviceConnectEventArgs))]
    [JsonSerializable(typeof(DeviceEventArgs))]
    [JsonSerializable(typeof(StreamDeckEventArgs))]
    [JsonSerializable(typeof(StreamDeckEventArgs<SettingsPayload>))]

    // Receiving - Action Specific.
    [JsonSerializable(typeof(ActionEventArgs<ActionPayload>))]
    [JsonSerializable(typeof(ActionEventArgs<KeyPayload>))]
    [JsonSerializable(typeof(ActionEventArgs<KeyPayload>))]
    [JsonSerializable(typeof(ActionEventArgs))]
    [JsonSerializable(typeof(ActionEventArgs))]
    [JsonSerializable(typeof(ActionEventArgs<JsonObject>))]
    [JsonSerializable(typeof(ActionEventArgs<TitlePayload>))]
    [JsonSerializable(typeof(ActionEventArgs<ActionPayload>))]
    [JsonSerializable(typeof(ActionEventArgs<ActionPayload>))]

    // Receiving - Plugin.
    [JsonSerializable(typeof(RegistrationInfo))]

    // Sending.
    [JsonSerializable(typeof(ActionMessage<JsonElement>))]
    [JsonSerializable(typeof(ContextMessage))]
    [JsonSerializable(typeof(ContextMessage<JsonElement>))]
    [JsonSerializable(typeof(ContextMessage<SetImagePayload>))]
    [JsonSerializable(typeof(ContextMessage<SetStatePayload>))]
    [JsonSerializable(typeof(ContextMessage<SetTitlePayload>))]
    [JsonSerializable(typeof(DeviceMessage<SwitchToProfilePayload>))]
    [JsonSerializable(typeof(Message<LogPayload>))]
    [JsonSerializable(typeof(Message<UrlPayload>))]
    [JsonSerializable(typeof(RegistrationParameters))]

    [ExcludeFromCodeCoverage]
    internal partial class StreamDeckJsonContext : JsonSerializerContext
    {
    }
}
