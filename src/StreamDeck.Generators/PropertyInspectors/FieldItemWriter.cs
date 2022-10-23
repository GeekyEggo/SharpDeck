namespace StreamDeck.Generators.Generators.PropertyInspectors
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.IO;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/item">sdpi-item</see>, with a nested input.
    /// </summary>
    internal class FieldItemWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldItemWriter"/> class.
        /// </summary>
        /// <param name="tagName">Name of the HTML tag.</param>
        public FieldItemWriter(string tagName)
            => this.TagName = tagName;

        /// <summary>
        /// Gets the name of the HTML tag.
        /// </summary>
        private string TagName { get; }

        /// <summary>
        /// Writes the component <paramref name="data"/> to the specified <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The parent HTML element to write to.</param>
        /// <param name="data">The <see cref="AttributeData"/> containing information about the component.</param>
        public virtual void Write(HtmlStringWriter parent, AttributeData data)
        {
            parent.Add("sdpi-item", item =>
            {
                item.AddAttribute("label", data.GetNamedArgumentValueOrDefault(nameof(InputAttribute.Label), () => string.Empty));
                this.WriteInput(item, data);
            });
        }

        /// <summary>
        /// Writes the input associated with the <paramref name="data"/>, to the specified <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The parent HTML element to write to.</param>
        /// <param name="data">The <see cref="AttributeData"/> containing information about the component.</param>
        protected virtual void WriteInput(HtmlStringWriter parent, AttributeData data)
            => parent.Add(this.TagName, elem => this.WriteAttributes(elem, data));

        /// <summary>
        /// Writes the attributes contained within <paramref name="data"/>, to the specified <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="data">The <see cref="AttributeData"/> containing information about the component.</param>
        private void WriteAttributes(HtmlStringWriter element, AttributeData data)
        {
            foreach (var attr in data.NamedArguments.Where(a => this.CanWriteProperty(a.Key, a.Value)))
            {
                var (key, value) = this.GetAttribute(attr.Key, attr.Value.Value);
                element.AddAttribute(key, value);
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
    }
}
