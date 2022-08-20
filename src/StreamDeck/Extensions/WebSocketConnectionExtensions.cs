namespace StreamDeck.Extensions
{
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using StreamDeck.Net;

    /// <summary>
    /// Provides extension methods for <see cref="IWebSocketConnection"/>.
    /// </summary>
    internal static class WebSocketConnectionExtensions
    {
        /// <summary>
        /// Sends the specified <paramref name="value"/> as a JSON message.
        /// </summary>
        /// <param name="value">The value to send.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        public static async Task SendAsync(this IWebSocketConnection connection, object value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value, options);
            await connection.SendAsync(json, cancellationToken);
        }
    }
}
