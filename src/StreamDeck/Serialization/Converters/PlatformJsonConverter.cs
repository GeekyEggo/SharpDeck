namespace StreamDeck.Serialization.Converters
{
    using System;
    using System.Text.Json;
    using StreamDeck.Events;

    /// <summary>
    /// Provides a JSON converter for <see cref="Platform"/>.
    /// </summary>
    internal class PlatformJsonConverter : EnumStringJsonConverter<Platform>
    {
        /// <inheritdoc/>
        public override Platform Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new Platform(reader.GetString());
    }
}
