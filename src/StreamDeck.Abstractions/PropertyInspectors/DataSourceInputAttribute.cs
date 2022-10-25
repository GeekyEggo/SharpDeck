namespace StreamDeck.PropertyInspectors
{
    /// <summary>
    /// Provides a base for all components that have a data source.
    /// </summary>
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    abstract class DataSourceInputAttribute : InputAttribute
    {
        /// <summary>
        /// Gets or sets the optional remote data source (<see href="https://sdpi-components.dev/docs/helpers/data-source">read more</see>).
        /// </summary>
        public string? DataSource { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sendToPropertyInspector is actively monitored allowing for the plugin to update the items.
        /// </summary>
        public bool HotReload { get; set; } = false;

        /// <summary>
        /// Gets or sets the text shown whilst the items are loaded.
        /// </summary>
        public string? Loading { get; set; }

        /// <summary>
        /// Gets or sets the preferred value type of the persisted setting; when 'boolean', 'false' and 0 will equate in false. Defaults to 'string'.
        /// </summary>
        public InputValueType ValueType { get; set; } = InputValueType.String;
    }
}
