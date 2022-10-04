namespace StreamDeck.Extensions.PropertyInspectors
{
    using StreamDeck.Extensions.Serialization;

    /// <summary>
    /// Provides extension methods for <see cref="StreamDeckAction"/>.
    /// </summary>
    public static class StreamDeckActionExtensions
    {
        /// <summary>
        /// Sends the data source to the property inspector of this instance asynchronously.
        /// </summary>
        /// <param name="action">The <see cref="StreamDeckAction" /> whose property inspector will receive the payload.</param>
        /// <param name="event">The event of the data source.</param>
        /// <param name="items">The items of the data source.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>
        /// The task of sending payload to the property inspector.
        /// </returns>
        /// <remarks>
        /// Read more about data sources <see href="https://sdpi-components.dev/docs/helpers/data-source" />.
        /// </remarks>
        public static Task SendDataSourceToPropertyInspectorAsync(this StreamDeckAction action, string @event, IEnumerable<DataSourceItem> items, CancellationToken cancellationToken = default)
            => action.SendToPropertyInspectorAsync(new DataSourceResponse(@event, items), StreamDeckJsonContext.Default.DataSourceResponse, cancellationToken);
    }
}
