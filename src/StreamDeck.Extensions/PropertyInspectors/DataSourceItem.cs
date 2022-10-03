namespace StreamDeck.Extensions.PropertyInspectors
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides information about an item within a data source payload.
    /// </summary>
    /// <remarks>
    /// Read more about data sources <see href="https://sdpi-components.dev/docs/helpers/data-source#payload-structure" />.
    /// </remarks>
    public struct DataSourceItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceItem"/> struct.
        /// </summary>
        public DataSourceItem()
            => this.Value = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceItem"/> class as an item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="label">The label.</param>
        /// <param name="disabled">The value indicating whether this instance is disabled.</param>
        public DataSourceItem(string value, string? label = null, bool disabled = false)
        {
            this.Disabled = disabled;
            this.Label = label;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceItem"/> class as an item group.
        /// </summary>
        /// <param name="children">The children.</param>
        public DataSourceItem(IEnumerable<DataSourceItem> children)
            : this(null!, children)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceItem"/> class as an item group.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="children">The children.</param>
        public DataSourceItem(string label, IEnumerable<DataSourceItem> children)
        {
            this.Children = children;
            this.Label = label;
        }

        /// <summary>
        /// Gets the optional children associated with the item.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<DataSourceItem>? Children { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is disabled.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Disabled { get; } = default;

        /// <summary>
        /// Gets the label.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Label { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Value { get; }
    }
}
