namespace StreamDeck.Generators.PropertyInspectors
{
    using StreamDeck.Generators.Generators.PropertyInspectors;
    using StreamDeck.PropertyInspectors;

    /// <summary>
    /// Provides an HTML writer capable of writing an <see href="https://sdpi-components.dev/docs/components/file">sdpi-file</see>.
    /// </summary>
    internal class FileInputWriter : InputWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextfieldInputWriter"/> class.
        /// </summary>
        public FileInputWriter()
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
