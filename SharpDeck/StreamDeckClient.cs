namespace SharpDeck
{
    using Events;
    using Newtonsoft.Json;
    using SharpDeck.Models;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    public class StreamDeckClient : IDisposable
    {
        static StreamDeckClient()
        {
            var _parent = typeof(StreamDeckClient);
            var events = _parent.GetEvents(BindingFlags.Instance | BindingFlags.Public);

            foreach (var ev in events)
            {
                var info = new StreamDeckEventInfo(ev, _parent.GetField(ev.Name, BindingFlags.NonPublic | BindingFlags.Instance));
                if (!string.IsNullOrWhiteSpace(info.Name))
                {
                    EventFactory.Add(info.Name, info);
                }
            }
        }

        public StreamDeckClient(string[] args)
            : this(RegistrationParameters.Parse(args))
        {
        }

        public StreamDeckClient(RegistrationParameters registrationParams)
        {
            this.WebSocketClient = new WebSocketClient("ws://localhost", registrationParams.Port);
            this.WebSocketClient.OnMessage += this.OnWebSocketClientMessage;
        }

        #region Events received

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

        #endregion

        private static IDictionary<string, StreamDeckEventInfo> EventFactory { get; }
        private RegistrationParameters RegistrationParameters { get; set; }
        private WebSocketClient WebSocketClient { get; }

        public Task StartAsync()
            => this.WebSocketClient.ConnectAsync();

        public void Dispose()
            => this.WebSocketClient?.Dispose();

        private void OnWebSocketClientMessage(object sender, string e)
        {
            var initialArgs = JsonConvert.DeserializeObject<StreamDeckEventArgs>(e);

            if (EventFactory.TryGetValue(initialArgs.Event, out var ev))
            {
                var eventArgs = JsonConvert.DeserializeObject(e, ev.ArgsType);
                ev.Invoke(this, eventArgs);
            }
        }
    }
}
