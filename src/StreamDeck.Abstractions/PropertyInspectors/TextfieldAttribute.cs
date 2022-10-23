namespace StreamDeck.PropertyInspectors
{
    /// <summary>
    /// Provides information about a <see href="https://sdpi-components.dev/docs/components/textfield">sdpi-textfield</see> to be rendered within a property inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class TextfieldAttribute : InputAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is required; when <c>true</c>, an icon is shown in the input if the value is empty.
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets maximum length of the value.
        /// </summary>
        public int MaxLength { get; set; } = default;

        /// <summary>
        /// Gets or sets regular expression used to validate the input.
        /// </summary>
        public string? Pattern { get; set; }

        /// <summary>
        /// Gets or sets the placeholder text shown in the input.
        /// </summary>
        public string? Placeholder { get; set; }
    }
}
