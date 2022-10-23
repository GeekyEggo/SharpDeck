namespace StreamDeck.Generators.PropertyInspectors
{
    using StreamDeck.Generators.Generators.PropertyInspectors;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/textfield">sdpi-textfield</see>.
    /// </summary>
    internal class TextfieldWriter : FieldItemWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextfieldWriter"/> class.
        /// </summary>
        public TextfieldWriter()
            : base("sdpi-textfield")
        {
        }

        /// <inheritdoc/>
        protected override (string PropertyName, object? Value) GetAttribute(string propertyName, object? value)
            => propertyName switch
            {
                nameof(TextfieldAttribute.IsRequired) => ("required", value),
                _ => base.GetAttribute(propertyName, value)
            };
    }
}
