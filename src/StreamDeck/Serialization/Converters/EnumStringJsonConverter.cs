namespace StreamDeck.Serialization.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides a base class for <see cref="EnumString{T}"/> JSON converters.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="EnumString{T}"/>.</typeparam>
    internal abstract class EnumStringJsonConverter<T> : JsonConverter<T>
        where T : EnumString<T>
    {
        /// <inheritdoc/>
        public abstract override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Value);
    }
}
