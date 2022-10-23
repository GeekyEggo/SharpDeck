namespace StreamDeck.Generators.PropertyInspectors
{
    using StreamDeck.Generators.Generators.PropertyInspectors;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/checkbox">sdpi-checkbox</see>.
    /// </summary>
    internal class CheckboxInputWriter : InputWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckboxInputWriter"/> class.
        /// </summary>
        public CheckboxInputWriter()
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
