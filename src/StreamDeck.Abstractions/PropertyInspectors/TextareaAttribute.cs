namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about a <see href="https://sdpi-components.dev/docs/components/textarea">sdpi-textarea</see> to be rendered within a property inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class TextareaAttribute : InputAttribute
    {
        /// <summary>
        /// Gets or sets maximum length of the value.
        /// </summary>
        public int MaxLength { get; set; } = default;

        /// <summary>
        /// Gets or sets the size, in rows, of the text area.
        /// </summary>
        public int Rows { get; set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the current length and maximum length are displayed.
        /// </summary>
        public bool ShowLength { get; set; } = false;
    }
}
