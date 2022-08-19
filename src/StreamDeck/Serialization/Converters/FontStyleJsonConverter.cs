namespace StreamDeck.Serialization.Converters
{
    using System;
    using System.Text.Json;
    using StreamDeck.Events.Received;

    /// <summary>
    /// Provides a JSON converter for <see cref="FontStyle"/>.
    /// </summary>
    internal class FontStyleJsonConverter : EnumStringJsonConverter<FontStyle>
    {
        /// <inheritdoc/>
        public override FontStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new FontStyle(reader.GetString());
    }
}
