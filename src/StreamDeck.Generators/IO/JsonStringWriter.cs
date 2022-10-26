namespace StreamDeck.Generators.IO
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides a basic JSON serialization; this can be used to prevent external requirements and dependencies.
    /// </summary>
    /// <remarks>Serialization is predominantly based on Json.NET's serialization guide (https://www.newtonsoft.com/json/help/html/SerializationGuide.htm).</remarks>
    internal sealed class JsonStringWriter : IDisposable
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="JsonStringWriter"/> class from being created.
        /// </summary>
        private JsonStringWriter()
        {
        }

        /// <summary>
        /// Gets the underlying JSON string.
        /// </summary>
        private IndentedStringWriter Json { get; } = new IndentedStringWriter();

        /// <summary>
        /// Gets the properties that can be serialized, indexed by their parent type.
        /// </summary>
        private IDictionary<Type, IEnumerable<JsonPropertyInfo>> SerializableProperties { get; } = new Dictionary<Type, IEnumerable<JsonPropertyInfo>>();

        /// <summary>
        /// Converts to <paramref name="value"/> to a camel-case string, e.g. "IsChecked" becomes "isChecked".
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The camel-case value.</returns>
        public static string ToCamalCase(string value)
        {
            // Ref: https://github.com/dotnet/runtime/blob/24813dc41dd3d573458e710292a109f8ffa4f415/src/libraries/System.Text.Json/Common/JsonCamelCaseNamingPolicy.cs#L32-L57
            var chars = value.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                // Stop when next char is already lowercase.
                if (i > 0 && (i + 1 < chars.Length) && !char.IsUpper(chars[i + 1]))
                {
                    // If the next char is a space, lowercase current char before exiting.
                    if (chars[i + 1] == ' ')
                    {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The JSON representation of <paramref name="value"/>.</returns>
        public static string Serialize(object value)
        {
            using var json = new JsonStringWriter();
            json.Write(value);

            return json.Json.ToString();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Json.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Determines whether the specified property value can be written.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value can be written; otherwise <c>false</c>.</returns>
        private bool CanWriteProperty(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is ICollection coll)
            {
                return coll.Count > 0;
            }

            return true;
        }

        /// <summary>
        /// Writes the specified <paramref name="value"/> to the JSON string.
        /// </summary>
        /// <param name="value">The value to write.</param>
        private void Write(object value)
        {
            switch (value)
            {
                // Null.
                case null:
                    this.Json.Write("null");
                    return;

                // Strings.
                case char _:
                case string _:
                case Guid _:
                case TimeSpan _:
                case Type _:
                    this.WriteString(value.ToString());
                    return;

                // String (extended)
                case byte[] byteArray:
                    this.WriteString(Convert.ToBase64String(byteArray));
                    return;

                // Dictionary (represented as objects with key.ToString() as the property).
                case IDictionary dictionary:
                    this.WriteDictionary(dictionary);
                    return;

                // Array'esk.
                case IEnumerable array:
                    this.WriteArray(array);
                    return;

                // Boolean.
                case bool boolean:
                    this.Json.Write(boolean.ToString().ToLower());
                    return;

                // Date times.
                case DateTime dateTime:
                    this.WriteString(dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                    return;

                // Number.
                case byte _:
                case sbyte _:
                case ushort _:
                case short _:
                case uint _:
                case int _:
                case long _:
                case ulong _:
                case float _:
                case double _:
                case decimal _:
                case IntPtr _:
                case UIntPtr _:
                    this.Json.Write(value);
                    return;
            }

            var type = value.GetType();

            // Enum.
            if (type.IsEnum)
            {
                this.Json.Write((int)value);
                return;
            }

            // Object.
            if (!type.IsPrimitive)
            {
                this.WriteObject(value);
                return;
            }

            // Fallback to string.
            this.WriteString(value.ToString());
        }

        /// <summary>
        /// Writes the specified <paramref name="array"/> to the JSON string.
        /// </summary>
        /// <param name="array">The array to write.</param>
        private void WriteArray(IEnumerable array)
        {
            this.WriteWrapped('[', ']', () =>
            {
                var enumerator = array.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    this.Write(enumerator.Current);
                }

                while (enumerator.MoveNext())
                {
                    this.Json.WriteLine(",");
                    this.Write(enumerator.Current);
                }
            });
        }

        /// <summary>
        /// Writes the specified <paramref name="dictionary"/> as an object to the JSON string.
        /// </summary>
        /// <param name="dictionary">The dictionary to write.</param>
        private void WriteDictionary(IDictionary dictionary)
        {
            var properties = dictionary
                .Keys
                .Cast<object>()
                .Select(k => new KeyValuePair<string, object>(k.ToString(), dictionary[k]));

            this.WriteObject(properties);
        }

        /// <summary>
        /// Writes the specified <paramref name="obj"/> to the JSON string.
        /// </summary>
        /// <param name="obj">The object value to write.</param>
        private void WriteObject(object obj)
        {
            var type = obj.GetType();
            if (type.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
            {
                return;
            }

            if (!this.SerializableProperties.TryGetValue(type, out var properties))
            {
                properties = type
                    .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute<IgnoreDataMemberAttribute>() == null)
                    .Select(p => new JsonPropertyInfo(p))
                    .OrderBy(p => p.Name)
                    .ToArray();

                this.SerializableProperties[type] = properties;
            }

            this.WriteObject(properties.Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(obj))));
        }

        /// <summary>
        /// Writes an object with the specified <paramref name="properties"/> to the JSON string.
        /// </summary>
        /// <param name="properties">The properties that represent the object.</param>
        private void WriteObject(IEnumerable<KeyValuePair<string, object>> properties)
        {
            this.WriteWrapped('{', '}', () =>
            {
                var validProperties = properties.Where(p => this.CanWriteProperty(p.Value)).GetEnumerator();
                if (validProperties.MoveNext())
                {
                    Write(validProperties.Current);
                }

                while (validProperties.MoveNext())
                {
                    this.Json.WriteLine(",");
                    Write(validProperties.Current);
                }
            });

            void Write(KeyValuePair<string, object> property)
            {
                this.WriteString(property.Key);
                this.Json.Write(": ");
                this.Write(property.Value);
            }
        }

        /// <summary>
        /// Executes <paramref name="action"/>, wrapped by the <see cref="open"/> and <see cref="close"/> tags.
        /// </summary>
        /// <param name="open">The open tag.</param>
        /// <param name="close">The close tag.</param>
        /// <param name="action">The action responsible for writing the JSON.</param>
        private void WriteWrapped(char open, char close, Action action)
        {
            this.Json.WriteLine(open.ToString());
            this.Json.Indent++;

            action();

            this.Json.Indent--;
            this.Json.WriteLine();
            this.Json.Write(close);
        }

        /// <summary>
        /// Writes the escaped <see cref="string"/> to the JSON string.
        /// </summary>
        /// <param name="value">The value.</param>
        private void WriteString(string value)
        {
            var escaped = new StringBuilder();
            foreach (var chr in value)
            {
                escaped.Append(chr switch
                {
                    '"' => "\\\"",
                    '\\' => "\\\\",
                    '\b' => "\\b",
                    '\f' => "\\f",
                    '\n' => "\\n",
                    '\r' => "\\r",
                    '\t' => "\\t",
                    '\u2028' => "\\u2028",
                    '\u2029' => "\\u2029",
                    _ => chr
                });
            }

            this.Json.Write($"\"{escaped}\"");
        }

        /// <summary>
        /// Provides information about a property being serialized.
        /// </summary>
        private struct JsonPropertyInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="JsonPropertyInfo"/> struct.
            /// </summary>
            /// <param name="propertyInfo">The property information.</param>
            public JsonPropertyInfo(PropertyInfo propertyInfo)
            {
                this.PropertyInfo = propertyInfo;
                this.Name = propertyInfo.GetCustomAttribute<DataMemberAttribute>()?.Name ?? propertyInfo.Name;
                this.IgnoreValue = propertyInfo.GetCustomAttribute<IgnoreDataMemberWhenAttribute>()?.Value;
            }

            /// <summary>
            /// Gets the property information.
            /// </summary>
            public PropertyInfo PropertyInfo { get; }

            /// <summary>
            /// Gets the name that represents the serialized value.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets the ignored value; when this matches the property's value, the item will not be serialized.
            /// </summary>
            public object? IgnoreValue { get; }

            /// <summary>
            /// Gets the value from the specified <paramref name="obj"/>.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>The value.</returns>
            public object GetValue(object obj)
            {
                var value = this.PropertyInfo.GetValue(obj);
                return this.IgnoreValue?.Equals(value) == true ? null! : value;
            }
        }
    }
}
