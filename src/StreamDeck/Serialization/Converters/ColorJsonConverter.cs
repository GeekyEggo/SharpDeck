namespace StreamDeck.Serialization.Converters
{
    using System;
    using System.Drawing;
    using System.Text.Json;

    /// <summary>
    /// Provides a <see cref="JsonConverter{T}"/> capable of deserializing <see cref="Color"/>.
    /// </summary>
    public class ColorJsonConverter : JsonConverter<Color?>
    {
        /// <inheritdoc/>
        public override Color? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var htmlColor = reader.GetString();

            if (string.IsNullOrEmpty(htmlColor)
                || htmlColor.Length == 0
                || htmlColor[0] != '#')
            {
                return null;
            }

            // RGB without alpha, e.g. #AABBCC.
            if (htmlColor.Length == 7)
            {
                return Color.FromArgb(
                    Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(3, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(5, 2), 16));
            }

            // RGB with alpha, e.g. #AABBCCFF.
            if (htmlColor.Length == 9)
            {
                return Color.FromArgb(
                    Convert.ToInt32(htmlColor.Substring(7, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(3, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(5, 2), 16));
            }

            throw new NotSupportedException($"Cannot deserialize color value: {htmlColor}");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Color? value, JsonSerializerOptions options)
            => throw new NotImplementedException();
    }
}
