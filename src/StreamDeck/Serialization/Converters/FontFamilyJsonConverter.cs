namespace StreamDeck.Serialization.Converters
{
    using System;
    using System.Text.Json;
    using StreamDeck.Events;

    /// <summary>
    /// Provides a JSON converter for <see cref="FontFamily"/>.
    /// </summary>
    internal class FontFamilyJsonConverter : EnumStringJsonConverter<FontFamily>
    {
        /// <inheritdoc/>
        public override FontFamily Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new FontFamily(reader.GetString());
    }
}
