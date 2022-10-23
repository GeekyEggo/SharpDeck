namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about a <see href="https://sdpi-components.dev/docs/components/checkbox">sdpi-checkbox</see> to be rendered within a property inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class CheckboxAttribute : InputAttribute
    {
        /// <summary>
        /// Gets or sets the optional label text shown to the right of the checkbox.
        /// </summary>
        public string? CheckboxLabel { get; set; }
    }
}
