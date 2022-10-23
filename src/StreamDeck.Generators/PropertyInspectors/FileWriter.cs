namespace StreamDeck.Generators.PropertyInspectors
{
    using StreamDeck.Generators.Generators.PropertyInspectors;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/file">sdpi-file</see>.
    /// </summary>
    internal class FileWriter : FieldItemWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextfieldWriter"/> class.
        /// </summary>
        public FileWriter()
            : base("sdpi-file")
        {
        }

        /// <inheritdoc/>
        protected override (string PropertyName, object? Value) GetAttribute(string propertyName, object? value)
            => propertyName switch
            {
                nameof(FileAttribute.ButtonLabel) => ("label", value),
                _ => base.GetAttribute(propertyName, value)
            };
    }
}
