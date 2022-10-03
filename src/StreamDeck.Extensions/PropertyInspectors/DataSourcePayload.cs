namespace StreamDeck.Extensions.PropertyInspectors
{
    /// <summary>
    /// A payload that provides a data source to the property inspector.
    /// </summary>
    public class DataSourcePayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourcePayload"/> class.
        /// </summary>
        /// <param name="event">The event that the payload is associated with.</param>
        /// <param name="items">The items.</param>
        public DataSourcePayload(string @event, params DataSourceItem[] items)
            : this(@event, items.AsEnumerable())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourcePayload"/> class.
        /// </summary>
        /// <param name="event">The event that the payload is associated with.</param>
        /// <param name="items">The items.</param>
        public DataSourcePayload(string @event, IEnumerable<DataSourceItem> items)
        {
            this.Event = @event;
            this.Items = items.ToList();
        }

        /// <summary>
        /// Gets the event that the payload is associated with.
        /// </summary>
        public string Event { get; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public List<DataSourceItem> Items { get; }
    }
}
