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
        protected override bool CanWriteProperty(string propertyName, object? value)
        {
            if (propertyName == nameof(TextfieldAttribute.MaxLength)
                && value is int maxLength)
            {
                return maxLength != TextfieldAttribute.ZERO_MAX_LENGTH;
            }

            return base.CanWriteProperty(propertyName, value);
        }

        /// <inheritdoc/>
        protected override string GetAttributeName(string propertyName)
            => propertyName switch
            {
                nameof(TextfieldAttribute.IsRequired) => "required",
                _ => base.GetAttributeName(propertyName)
            };
    }
}
