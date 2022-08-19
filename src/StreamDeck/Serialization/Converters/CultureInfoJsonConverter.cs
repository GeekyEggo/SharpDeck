namespace StreamDeck.Serialization.Converters
{
    using System;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides a JSON converter for <see cref="CultureInfo"/>.
    /// </summary>
    public class CultureInfoJsonConverter : JsonConverter<CultureInfo>
    {
        /// <inheritdoc/>
        public override CultureInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var name = reader.GetString();
                if (name != null)
                {
                    return CultureInfo.GetCultureInfo(name);
                }
                else
                {
                    return CultureInfo.CurrentCulture;
                }
            }
            catch
            {
                return CultureInfo.CurrentCulture;
            }
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Name);
    }
}
