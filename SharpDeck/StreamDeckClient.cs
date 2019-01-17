namespace SharpDeck
{
    using Events;
    using Newtonsoft.Json;
    using SharpDeck.Models;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides events and methods that allow for communication with an Elgato Stream Deck.
    /// </summary>
    public class StreamDeckClient : IDisposable
    {
        /// <summary>
        /// Initializes static members of the <see cref="StreamDeckClient"/> class.
        /// </summary>
        static StreamDeckClient()
        {
            var typeOfThis = typeof(StreamDeckClient);
            var events = typeOfThis.GetEvents(BindingFlags.Instance | BindingFlags.Public);

            foreach (var ev in events)
            {
                var info = new StreamDeckEventInfo(ev, typeOfThis.GetField(ev.Name, BindingFlags.NonPublic | BindingFlags.Instance));
                if (!string.IsNullOrWhiteSpace(info.Name))
                {
                    EventFactory.Add(info.Name, info);
                }
            }
        }

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
            this.WebSocket = new ClientWebSocketWrapper($"ws://localhost:{regParams.Port}/");
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
        internal IWebSocket WebSocket { get; set; }

        /// <summary>
        /// Gets the event factory cache; this is used to determine which event should be triggered when a message is received.
        /// </summary>
        private static IDictionary<string, StreamDeckEventInfo> EventFactory { get; } = new Dictionary<string, StreamDeckEventInfo>();

        /// <summary>
        /// Gets or sets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; set; }

        /// <summary>
        /// Starts the client.
        /// </summary>
        public void Start()
            => this.WebSocket.Connect();

        /// <summary>
        /// Stops the client.
        /// </summary>
        public void Stop()
            => this.WebSocket.Close();

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
            // attempt to determine which event was received
            StreamDeckEventArgs eventArgs;
            try
            {
                eventArgs = JsonConvert.DeserializeObject<StreamDeckEventArgs>(e.Message);
            }
            catch
            {
                this.OnError?.Invoke(this, new StreamDeckClientErrorEventArgs("Unable to parse the message supplied by the Stream Deck"));
                return;
            }

            // determine if we can handle the event
            if (EventFactory.TryGetValue(eventArgs.Event, out var ev))
            {
                var args = JsonConvert.DeserializeObject(e.Message, ev.ArgsType);
                ev.Invoke(this, args);
            }
            else
            {
                this.OnError?.Invoke(this, new StreamDeckClientErrorEventArgs($"Stream Deck supplied an unspported event: {eventArgs.Event}"));
            }
        }
    }
}
