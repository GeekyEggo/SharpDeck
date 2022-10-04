namespace StreamDeck.Extensions.PropertyInspectors
{
    using StreamDeck.Extensions.Serialization;

    /// <summary>
    /// Provides extension methods for <see cref="IStreamDeckConnection"/>.
    /// </summary>
    public static class StreamDeckConnectionExtensions
    {
        /// <summary>
        /// Sends the data source to the property inspector asynchronously.
        /// </summary>
        /// <param name="connection">The <see cref="IStreamDeckConnection" />.</param>
        /// <param name="context">The context of the action that will receive the payload.</param>
        /// <param name="action">The action whose property inspector will receive the payload.</param>
        /// <param name="event">The event of the data source.</param>
        /// <param name="items">The items of the data source.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>
        /// The task of sending payload to the property inspector.
        /// </returns>
        /// <remarks>
        /// Read more about data sources <see href="https://sdpi-components.dev/docs/helpers/data-source" />.
        /// </remarks>
        public static Task SendDataSourceToPropertyInspectorAsync(this IStreamDeckConnection connection, string context, string action, string @event, IEnumerable<DataSourceItem> items, CancellationToken cancellationToken = default)
            => connection.SendToPropertyInspectorAsync(context, action, new DataSourceResponse(@event, items), StreamDeckJsonContext.Default.DataSourceResponse, cancellationToken);
    }
}
