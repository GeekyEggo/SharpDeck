namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about a <see href="https://sdpi-components.dev/docs/components/radio">sdpi-radio</see> to be rendered within a property inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class RadioAttribute : DataSourceInputAttribute
    {
        /// <summary>
        /// Gets or sets the number of columns to render the inputs in; valid values are 1-6.
        /// </summary>
        public int Columns { get; set; } = default;
    }
}
