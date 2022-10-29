namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about a <see href="https://sdpi-components.dev/docs/components/checkbox-list">sdpi-checkbox-list</see> to be rendered within a property inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class CheckboxListAttribute : DataSourceInputAttribute
    {
        /// <summary>
        /// Gets or sets the number of columns to render the inputs in; valid values are 1-6.
        /// </summary>
        public int Columns { get; set; } = default;
    }
}
