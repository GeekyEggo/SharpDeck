namespace StreamDeck.Generators.PropertyInspectors
{
    using StreamDeck.Generators.Generators.PropertyInspectors;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/range">sdpi-range</see>.
    /// </summary>
    internal class RangeInputWriter : InputWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeInputWriter"/> class.
        /// </summary>
        public RangeInputWriter()
            : base("sdpi-range")
        {
        }

        /// <inheritdoc/>
        protected override bool CanWriteProperty(string propertyName, object? value)
            => propertyName == nameof(RangeAttribute.Min)
            || propertyName == nameof(RangeAttribute.Max)
            || base.CanWriteProperty(propertyName, value);
    }
}
