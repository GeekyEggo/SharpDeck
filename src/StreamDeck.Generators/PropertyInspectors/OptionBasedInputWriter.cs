namespace StreamDeck.Generators.PropertyInspectors
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Generators.Extensions;
    using StreamDeck.Generators.IO;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/checkbox-list">sdpi-checkbox-list</see> or
    /// <see href="https://sdpi-components.dev/docs/components/radio">sdpi-radio</see>. This also acts as a base class for
    /// <see href="https://sdpi-components.dev/docs/components/select">sdpi-select</see>, which is fulfilled by <see cref="SelectInputWriter"/> that enables grouping.
    /// </summary>
    internal class OptionBasedInputWriter : InputWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionBasedInputWriter"/> class.
        /// </summary>
        /// <param name="tagName">Name of the HTML tag.</param>
        public OptionBasedInputWriter(string tagName)
            : base(tagName)
        {
        }

        /// <inheritdoc/>
        protected override (string PropertyName, object? Value) GetAttribute(string propertyName, object? value)
            => propertyName switch
            {
                nameof(DataSourceInputAttribute.HotReload) => ("hot-reload", value),
                nameof(DataSourceInputAttribute.ValueType) when value is int and (int)InputValueType.Boolean => ("value-type", "boolean"),
                nameof(DataSourceInputAttribute.ValueType) when value is int and (int)InputValueType.Number => ("value-type", "number"),
                nameof(DataSourceInputAttribute.ValueType) => ("value-type", "string"),
                _ => base.GetAttribute(propertyName, value)
            };

        /// <inheritdoc/>
        protected override void WriteInput(HtmlStringWriter parent, AttributeData data, ImmutableArray<AttributeData> propertyAttributes)
        {
            parent.Add(this.TagName, select =>
            {
                this.WriteAttributes(select, data);
                foreach (var (option, _) in this.GetOptions(propertyAttributes))
                {
                    select.Add(option);
                }
            });
        }

        /// <summary>
        /// Gets the options from the <see cref="OptionAttribute"/> defined within the <paramref name="propertyAttributes"/>.
        /// </summary>
        /// <param name="propertyAttributes">The collection of attributes defined on the property.</param>
        /// <returns>The collection of options, and their data that generated them.</returns>
        protected virtual IEnumerable<(HtmlStringWriter Element, AttributeData Data)> GetOptions(ImmutableArray<AttributeData> propertyAttributes)
        {
            foreach (var attr in propertyAttributes.Where(p => p.AttributeClass?.ToFullNameString() == typeof(OptionAttribute).FullName))
            {
                var option = new HtmlStringWriter("option")
                    .AddAttribute("value", attr.GetNamedArgumentValueOrDefault(nameof(OptionAttribute.Value), string.Empty))
                    .AddAttribute("disabled", attr.GetNamedArgumentValueOrDefault(nameof(OptionAttribute.IsDisabled), false))
                    .SetInnerText(attr.GetNamedArgumentValueOrDefault(nameof(OptionAttribute.Label), string.Empty));

                yield return (option, attr);
            }
        }
    }
}
