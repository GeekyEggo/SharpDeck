namespace StreamDeck.Extensions
{
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization.Metadata;
    using StreamDeck.Events;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides extension methods for <see cref="StreamDeckEventArgs{TPayload}"/>.
    /// </summary>
    public static class StreamDeckEventArgsExtensions
    {
        /// <summary>
        /// Gets the payload as the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The desired type of the settings.</typeparam>
        /// <param name="args">The <see cref="StreamDeckEventArgs{JsonObject}"/> to get the payload from.</param>
        /// <param name="jsonTypeInfo">The JSON type information.</param>
        /// <returns>The settings as <typeparamref name="T"/>.</returns>
        public static T? GetPayload<T>(this StreamDeckEventArgs<JsonObject>? args, JsonTypeInfo<T>? jsonTypeInfo = null)
            where T : class
            => jsonTypeInfo == null
                ? args?.Payload?.Deserialize<T>(StreamDeckJsonContext.Default.Options)
                : args?.Payload?.Deserialize(jsonTypeInfo);
    }
}
