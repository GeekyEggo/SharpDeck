namespace StreamDeck.Extensions.PropertyInspectors
{
    /// <summary>
    /// Provides a payload that represents the request for a data source.
    /// </summary>
    public class DataSourceRequest
    {
        /// <summary>
        /// Gets or sets the event of the data source.
        /// </summary>
        public string Event { get; set; } = string.Empty;
    }
}
