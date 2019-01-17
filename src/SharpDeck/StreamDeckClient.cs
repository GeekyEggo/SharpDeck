namespace SharpDeck
{
    using Events;
    using SharpDeck.Models;
    using SharpDeck.Net;
    using System;

    /// <summary>
    /// Provides events and methods that allow for communication with an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckClient : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckClient"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public StreamDeckClient(string[] args)
            : this(RegistrationParameters.Parse(args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckClient"/> class.
        /// </summary>
        /// <param name="regParams">The registration parameters.</param>
        public StreamDeckClient(RegistrationParameters regParams)
        {
            this.WebSocket = new ClientWebSocketWrapper($"ws://localhost:{regParams.Port}");
            this.WebSocket.OnMessage += this.OnWebSocketClientMessage;
        }

        /// <summary>
        /// Occurs when the client encounters an error.
        /// </summary>
        public EventHandler<StreamDeckClientErrorEventArgs> OnError;

        /// <summary>
        /// Occurs when a monitored application is launched.
        /// </summary>
        [StreamDeckEvent("applicationDidLaunch")]
        public event EventHandler<ApplicationEventArgs> ApplicationDidLaunch;

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// </summary>
        [StreamDeckEvent("applicationDidTerminate")]
        public event EventHandler<ApplicationEventArgs> ApplicationDidTerminate;

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// </summary>
        [StreamDeckEvent("deviceDidConnect")]
        public event EventHandler<DeviceConnectEventArgs> DeviceDidConnect;

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// </summary>
        [StreamDeckEvent("deviceDidDisconnect")]
        public event EventHandler<DeviceEventArgs> DeciceDidDisconnect;

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        [StreamDeckEvent("keyDown")]
        public event EventHandler<KeyActionEventArgs> KeyDown;

        /// <summary>
        /// Occurs when the user releases a key.
        /// </summary>
        [StreamDeckEvent("keyUp")]
        public event EventHandler<KeyActionEventArgs> KeyUp;

        /// <summary>
        /// Occurs when the user changes the title or title parameters.
        /// </summary>
        [StreamDeckEvent("titleParametersDidChange")]
        public event EventHandler<TitleActionEventArgs> TitleParametersDidChange;

        /// <summary>
        /// Occurs when an instance of an action appears.
        /// </summary>
        [StreamDeckEvent("willAppear")]
        public event EventHandler<ActionEventArgs<ActionPayload>> WillAppear;

        /// <summary>
        /// Occurs when an instance of an action disappears.
        /// </summary>
        [StreamDeckEvent("willDisappear")]
        public event EventHandler<ActionEventArgs> WillDisappear;

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        internal IWebSocket WebSocket { get; }

        /// <summary>
        /// Gets or sets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; set; }

        /// <summary>
        /// Starts the client.
        /// </summary>
        public async void Start()
            => await this.WebSocket.ConnectAsync();

        /// <summary>
        /// Stops the client.
        /// </summary>
        public async void Stop()
            => await this.WebSocket.DisconnectAsync();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
            => this.WebSocket?.Dispose();

        /// <summary>
        /// Called when <see cref="IWebSocket.OnMessage"/> is triggered.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnWebSocketClientMessage(object sender, WebSocketMessageEventArgs e)
        {
            try
            {
                if (StreamDeckEventFactory.TryParse(e, out var ev, out var args))
                {
                    ev.Invoke(this, args);
                    this.WebSocket.SendAsync("Thank you for the tasty msg");
                }
            }
            catch (Exception ex)
            {
                this.OnError?.Invoke(this, new StreamDeckClientErrorEventArgs(ex.Message));
            }
        }
    }
}
