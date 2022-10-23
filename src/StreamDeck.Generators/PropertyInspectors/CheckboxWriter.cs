namespace StreamDeck.Generators.PropertyInspectors
{
    using StreamDeck.Generators.Generators.PropertyInspectors;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/checkbox">sdpi-checkbox</see>.
    /// </summary>
    internal class CheckboxWriter : FieldItemWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckboxWriter"/> class.
        /// </summary>
        public CheckboxWriter()
            : base("sdpi-checkbox")
        {
        }

        /// <inheritdoc/>
        protected override (string PropertyName, object? Value) GetAttribute(string propertyName, object? value)
            => propertyName switch
            {
                nameof(CheckboxAttribute.CheckboxLabel) => ("label", value),
                _ => base.GetAttribute(propertyName, value)
            };
    }
}
