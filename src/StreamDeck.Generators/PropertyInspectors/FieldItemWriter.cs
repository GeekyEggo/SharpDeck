namespace StreamDeck.Generators.Generators.PropertyInspectors
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.Serialization;
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
        /// Writes the component <paramref name="data"/> to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="data">The <see cref="AttributeData"/> containing information about the component.</param>
        public virtual void Write(HtmlTextWriter writer, AttributeData data)
        {
            writer.RenderBeginTag("sdpi-item");
            writer.AddAttribute("label", data.GetNamedArgumentValueOrDefault(nameof(InputAttribute.Label), () => string.Empty));

            this.WriteInput(writer, data);

            writer.RenderEndTag();
        }

        /// <summary>
        /// Writes the input associated with the <paramref name="data"/>, to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="data">The <see cref="AttributeData"/> containing information about the component.</param>
        protected virtual void WriteInput(HtmlTextWriter writer, AttributeData data)
        {
            writer.RenderBeginTag(this.TagName);
            foreach (var attr in data.NamedArguments.Where(a => this.CanWriteProperty(a.Key, a.Value)))
            {
                writer.AddAttribute(this.GetAttributeName(attr.Key), attr.Value.Value);
            }

            writer.RenderEndTag();
        }

        /// <summary>
        /// Determines whether this instance can write the specified property for the input.
        /// </summary>
        /// <param name="propertyName">The name of the property that represents the attribute.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> when the property can be written as an attribute; otherwise <c>false</c>.</returns>
        protected virtual bool CanWriteProperty(string propertyName, object? value)
            => propertyName != nameof(InputAttribute.Label);

        /// <summary>
        /// Gets the transformed name of the attribute.
        /// </summary>
        /// <param name="propertyName">The name of the property that represents the attribute.</param>
        /// <returns>The transformed name.</returns>
        protected virtual string GetAttributeName(string propertyName)
            => propertyName switch
            {
                nameof(InputAttribute.IsGlobal) => "global",
                _ => propertyName.ToLowerInvariant()
            };
    }
}
