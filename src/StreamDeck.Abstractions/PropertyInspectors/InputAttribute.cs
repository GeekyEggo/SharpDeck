namespace StreamDeck.PropertyInspectors
{
    /// <summary>
    /// Provides a base for all components with an input.
    /// </summary>
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    abstract class InputAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether the input is disabled.
        /// </summary>
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the value will be persisted to the global settings.
        /// </summary>
        public bool IsGlobal { get; set; } = false;

        /// <summary>
        /// Gets or sets the label text that represents the field; when clicked, the first input within the component gains focus.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Gets or sets the path of the property where the value should be persisted in the settings.
        /// </summary>
        public string? Setting { get; set; }
    }
}
