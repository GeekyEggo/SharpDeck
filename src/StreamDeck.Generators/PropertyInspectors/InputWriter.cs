namespace StreamDeck.Generators.PropertyInspectors
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.IO;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/item">sdpi-item</see>, with a nested input.
    /// </summary>
    internal class InputWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputWriter"/> class.
        /// </summary>
        /// <param name="tagName">Name of the HTML tag.</param>
        public InputWriter(string tagName)
            => this.TagName = tagName;

        /// <summary>
        /// Gets the name of the HTML tag.
        /// </summary>
        protected string TagName { get; }

        /// <summary>
        /// Writes the component <paramref name="data"/> to the specified <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The parent HTML element to write to.</param>
        /// <param name="context">The <see cref="PropertyInspectorPropertyContext"/> that contains information about the property and component being written.</param>
        public virtual void Write(HtmlStringWriter parent, PropertyInspectorPropertyContext context)
        {
            parent.Add("sdpi-item", item =>
            {
                item.AddAttribute("label", context.Component.GetNamedArgumentValueOrDefault(nameof(InputAttribute.Label), string.Empty));
                this.WriteInput(item, context);
            });
        }

        /// <summary>
        /// Writes the input associated with the <paramref name="data"/>, to the specified <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The parent HTML element to write to.</param>
        /// <param name="context">The <see cref="PropertyInspectorPropertyContext"/> that contains information about the property and component being written.</param>
        protected virtual void WriteInput(HtmlStringWriter parent, PropertyInspectorPropertyContext context)
            => parent.Add(this.TagName, elem => this.WriteAttributes(elem, context));

        /// <summary>
        /// Writes the attributes contained within <paramref name="data"/>, to the specified <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="context">The <see cref="PropertyInspectorPropertyContext"/> that contains information about the property and component being written.</param>
        protected void WriteAttributes(HtmlStringWriter element, PropertyInspectorPropertyContext context)
        {
            var isSettingDefined = false;
            foreach (var attr in context.Component.NamedArguments.Where(a => this.CanWriteProperty(a.Key, a.Value)))
            {
                var (key, value) = this.GetAttribute(attr.Key, attr.Value.Value);
                element.AddAttribute(key, value);

                if (attr.Key == nameof(InputAttribute.Setting))
                {
                    isSettingDefined = true;
                }
            }

            if (!isSettingDefined)
            {
                this.GenerateAndWriteSetting(element, context);
            }
        }

        /// <summary>
        /// Determines whether this instance can write the specified property for the input.
        /// </summary>
        /// <param name="propertyName">The name of the property that represents the attribute.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> when the property can be written as an attribute; otherwise <c>false</c>.</returns>
        protected virtual bool CanWriteProperty(string propertyName, object? value)
            => value is int intValue
                ? intValue > default(int)
                : propertyName != nameof(InputAttribute.Label);

        /// <summary>
        /// Gets the transformed name and value of the attribute.
        /// </summary>
        /// <param name="propertyName">The name of the property that represents the attribute.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The transformed name and value.</returns>
        protected virtual (string PropertyName, object? Value) GetAttribute(string propertyName, object? value)
            => propertyName switch
            {
                nameof(InputAttribute.IsDisabled) => ("disabled", value),
                nameof(InputAttribute.IsGlobal) => ("global", value),
                _ => (propertyName.ToLowerInvariant(), value)
            };

        /// <summary>
        /// Generates the "setting" attribute from the <paramref name="context"/>.
        /// </summary>
        /// <param name="element">The to element to write the "setting" attribute to.</param>
        /// <param name="context">The <see cref="PropertyInspectorPropertyContext"/> that contains information about the property and component being written.</param>
        private void GenerateAndWriteSetting(HtmlStringWriter element, PropertyInspectorPropertyContext context)
        {
            if (context.Property.TryGetOnlyValueOfAttribute("System.Text.Json.Serialization.JsonPropertyNameAttribute", out var value)
                && value is string jsonPropertyName
                && !string.IsNullOrWhiteSpace(jsonPropertyName))
            {
                element.AddAttribute("setting", jsonPropertyName);
            }
            else
            {
                element.AddAttribute("setting", JsonStringWriter.ToCamalCase(context.Property.Name));
            }
        }
    }
}
