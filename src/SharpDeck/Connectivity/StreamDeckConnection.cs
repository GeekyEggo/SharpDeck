namespace SharpDeck.Connectivity
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using SharpDeck.Events.Received;
    using SharpDeck.Events.Sent;
    using SharpDeck.Exceptions;
    using SharpDeck.Extensions;

    /// <summary>
    /// Provides a connection between Elgato Stream Deck devices and a Stream Deck client.
    /// </summary>
    internal sealed class StreamDeckConnection : IDisposable
    {
        /// <summary>
        /// Gets the event method cache, providing the delegates for each event triggered by Stream Deck.
        /// </summary>
        private static Lazy<IReadOnlyDictionary<string, MethodInfo>> StreamDeckClientEventRaiserCache { get; } = new Lazy<IReadOnlyDictionary<string, MethodInfo>>(GetStreamDeckClientEventRaisers, true);

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckConnection"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="registrationParameters">The registration parameters.</param>
        public StreamDeckConnection(StreamDeckClient client, RegistrationParameters registrationParameters)
        {
            this.Client = client;

            this.RegistrationParameters = registrationParameters;
            this.WebSocket = new WebSocketConnection($"ws://localhost:{registrationParameters.Port}/", Constants.DEFAULT_JSON_SETTINGS);
            this.WebSocket.MessageReceived += this.WebSocket_MessageReceived;
        }

        /// <summary>
        /// Gets the Stream Deck client associated with the connection.
        /// </summary>
        private StreamDeckClient Client { get; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; }

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        private WebSocketConnection WebSocket { get; set; }

        /// <summary>
        /// Initiates a connection to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await this.WebSocket.ConnectAsync();
            await this.WebSocket.SendJsonAsync(new RegistrationMessage(this.RegistrationParameters.Event, this.RegistrationParameters.PluginUUID));
            await this.WebSocket.ReceiveAsync(cancellationToken);
        }

        /// <summary>
        /// Disconnects the connection asynchronously.
        /// </summary>
        /// <returns>The task of disconnecting</returns>
        public Task DisconnectAsync()
            => this.WebSocket.DisconnectAsync();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.WebSocket?.DisconnectAsync();
            this.WebSocket = null;
        }

        /// <summary>
        /// Sends the value to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The task of sending the value.</returns>
        public Task SendAsync(object value)
            => this.WebSocket.SendJsonAsync(value);

        /// <summary>
        /// Handles the <see cref="WebSocketConnection.MessageReceived"/> event of <see cref="WebSocket"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs"/> instance containing the event data.</param>
        private void WebSocket_MessageReceived(object sender, WebSocketMessageEventArgs e)
        {
            try
            {
                // determine if there is an event specified, and a delegate handler
                var jArgs = JObject.Parse(e.Message);
                if (!this.TryGetEventHandler(jArgs, out var @delegate))
                {
                    this.Client.LogMessageAsync($"Unrecognised event received from Elgato Stream Deck: {e.Message}");
                    return;
                }

                // invoke the handler on the parent client
                using (SynchronizationContextSwitcher.NoContext())
                {
                    @delegate.Invoke(this.Client, new[] { jArgs.ToObject(@delegate.GetParameters()[0].ParameterType) });
                }
            }
            catch (Exception ex)
            {
                // otherwise simply invoke the error
                this.Client.OnError(new StreamDeckClientErrorEventArgs(ex, e?.Message));
            }
        }

        /// <summary>
        /// Attempts to get the delegate responsible for handling the event associated with the specified <paramref name="jArgs"/>.
        /// </summary>
        /// <param name="jArgs">The arguments supplied by the web socket message.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><c>true</c> when a handler was found; otherwise <c>false</c>.</returns>
        private bool TryGetEventHandler(JObject jArgs, out MethodInfo @delegate)
        {
            return jArgs.TryGetString(nameof(StreamDeckEventArgs.Event), out var @event)
                & StreamDeckClientEventRaiserCache.Value.TryGetValue(@event, out @delegate);
        }

        /// <summary>
        /// Gets the event method cache containing a dictionary of event names, as triggered by Stream Deck, as their associated delegate.
        /// </summary>
        /// <returns>The event method cache.</returns>
        private static IReadOnlyDictionary<string, MethodInfo> GetStreamDeckClientEventRaisers()
        {
            var cache = new Dictionary<string, MethodInfo>();
            foreach (var methodInfo in typeof(StreamDeckClient).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                // only add methods that have the attribute, accept a single parameter, and return a task
                var attr = methodInfo.GetCustomAttribute<StreamDeckEventAttribute>();
                if (attr != null && methodInfo.GetParameters().Length == 1 && methodInfo.ReturnType == typeof(Task))
                {
                    cache.Add(attr.Event, methodInfo);
                }
            }

            return cache;
        }
    }
}
