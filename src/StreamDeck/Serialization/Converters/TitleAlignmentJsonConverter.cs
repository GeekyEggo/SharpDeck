namespace StreamDeck.Serialization.Converters
{
    using System;
    using System.Text.Json;
    using StreamDeck.Events;

    /// <summary>
    /// Provides a JSON converter for <see cref="TitleAlignment"/>.
    /// </summary>
    internal class TitleAlignmentJsonConverter : EnumStringJsonConverter<TitleAlignment>
    {
        /// <inheritdoc/>
        public override TitleAlignment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new TitleAlignment(reader.GetString());
    }
}
