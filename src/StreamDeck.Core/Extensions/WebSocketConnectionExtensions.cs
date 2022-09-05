namespace StreamDeck.Extensions
{
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using System.Threading;
    using System.Threading.Tasks;
    using StreamDeck.Net;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides extension methods for <see cref="IWebSocketConnection"/>.
    /// </summary>
    internal static class WebSocketConnectionExtensions
    {
        /// <summary>
        /// Sends the specified <paramref name="value"/> as a JSON message.
        /// </summary>
        /// <param name="connection">The <see cref="IWebSocketConnection"/> to send the message to.</param>
        /// <param name="value">The value to send.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        internal static async Task SendAsync<T>(this IWebSocketConnection connection, T value, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value, StreamDeckJsonContext.Default.Options);
            await connection.SendAsync(json, cancellationToken);
        }

        /// <summary>
        /// Sends the specified <paramref name="value"/> as a JSON message.
        /// </summary>
        /// <param name="connection">The <see cref="IWebSocketConnection"/> to send the message to.</param>
        /// <param name="value">The value to send.</param>
        /// <param name="jsonTypeInfo">The JSON type information used to serialize <paramref name="value"/>.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        internal static async Task SendAsync<T>(this IWebSocketConnection connection, T value, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value, jsonTypeInfo);
            await connection.SendAsync(json, cancellationToken);
        }
    }
}
