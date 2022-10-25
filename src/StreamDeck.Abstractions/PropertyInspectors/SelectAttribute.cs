namespace StreamDeck.PropertyInspectors
{
    using System;

    /// <summary>
    /// Provides information about a <see href="https://sdpi-components.dev/docs/components/select">sdpi-select</see> to be rendered within a property inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class SelectAttribute : DataSourceInputAttribute
    {
        /// <summary>
        /// Gets or sets the optional placeholder text shown in the input.
        /// </summary>
        public string? Placeholder { get; set; }
    }
}
