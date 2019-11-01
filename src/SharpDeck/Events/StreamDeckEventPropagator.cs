namespace SharpDeck.Events
{
    using System;
    using System.Threading.Tasks;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides a base implementation containing events that can be propogated from a Stream Deck.
    /// </summary>
    public class StreamDeckEventPropagator : StreamDeckActionEventPropagator, IStreamDeckEventPropagator
    {
        /// <summary>
        /// Occurs when a monitored application is launched.
        /// </summary>
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidLaunch;

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// </summary>
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidTerminate;

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// </summary>
        public event EventHandler<DeviceConnectEventArgs> DeviceDidConnect;

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// </summary>
        public event EventHandler<DeviceEventArgs> DeviceDidDisconnect;

        /// <summary>
        /// Occurs when <see cref="IStreamDeckSender.GetGlobalSettingsAsync()"/> has been called to retrieve the persistent global data stored for the plugin.
        /// </summary>
        public event EventHandler<StreamDeckEventArgs<SettingsPayload>> DidReceiveGlobalSettings;

        /// <summary>
        /// Occurs when the computer is woken up.
        /// </summary>
        /// <remarks>
        /// A plugin may receive multiple <see cref="SystemDidWakeUp"/> events when waking up the computer.
        /// When the plugin receives the <see cref="SystemDidWakeUp"/> event, there is no garantee that the devices are available.
        /// </remarks>
        public event EventHandler<StreamDeckEventArgs> SystemDidWakeUp;

        /// <summary>
        /// Occurs when a monitored application is launched.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs{ApplicationPayload}"/> instance containing the event data.</param>
        internal Task OnApplicationDidLaunch(StreamDeckEventArgs<ApplicationPayload> args)
            => this.InvokeAsync(this.ApplicationDidLaunch, args);

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs{ApplicationPayload}"/> instance containing the event data.</param>
        internal Task OnApplicationDidTerminate(StreamDeckEventArgs<ApplicationPayload> args)
            => this.InvokeAsync(this.ApplicationDidTerminate, args);

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// </summary>
        /// <param name="args">The <see cref="DeviceConnectEventArgs"/> instance containing the event data.</param>
        internal Task OnDeviceDidConnect(DeviceConnectEventArgs args)
            => this.InvokeAsync(this.DeviceDidConnect, args);

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// </summary>
        /// <param name="args">The <see cref="DeviceEventArgs"/> instance containing the event data.</param>
        internal Task OnDeviceDidDisconnect(DeviceEventArgs args)
            => this.InvokeAsync(this.DeviceDidDisconnect, args);

        /// <summary>
        /// Raises the <see cref="DidReceiveGlobalSettings" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs{SettingsPayload}"/> instance containing the event data.</param>
        internal Task OnDidReceiveGlobalSettings(StreamDeckEventArgs<SettingsPayload> args)
            => this.InvokeAsync(this.DidReceiveGlobalSettings, args);

        /// <summary>
        /// Raises the <see cref="SystemDidWakeUp" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StreamDeckEventArgs"/> instance containing the event data.</param>
        internal Task OnSystemDidWakeUp(StreamDeckEventArgs args)
            => this.InvokeAsync(this.SystemDidWakeUp, args);

        /// <summary>
        /// Registers this instance to listen to events from <paramref name="source"/>, and propagate them.
        /// </summary>
        /// <param name="source">The source to listen to.</param>
        protected void PropagateFrom(StreamDeckEventPropagator source)
        {
            // global
            source.ApplicationDidLaunch     += (_, e) => this.OnApplicationDidLaunch(e);
            source.ApplicationDidTerminate  += (_, e) => this.OnApplicationDidTerminate(e);
            source.DeviceDidConnect         += (_, e) => this.OnDeviceDidConnect(e);
            source.DeviceDidDisconnect      += (_, e) => this.OnDeviceDidDisconnect(e);
            source.DidReceiveGlobalSettings += (_, e) => this.OnDidReceiveGlobalSettings(e);
            source.SystemDidWakeUp          += (_, e) => this.OnSystemDidWakeUp(e);

            // action specific
            source.DidReceiveSettings            += (_, e) => this.OnDidReceiveSettings(e);
            source.KeyDown                       += (_, e) => this.OnKeyDown(e);
            source.KeyUp                         += (_, e) => this.OnKeyUp(e);
            source.PropertyInspectorDidAppear    += (_, e) => this.OnPropertyInspectorDidAppear(e);
            source.PropertyInspectorDidDisappear += (_, e) => this.OnPropertyInspectorDidDisappear(e);
            source.SendToPlugin                  += (_, e) => this.OnSendToPlugin(e);
            source.TitleParametersDidChange      += (_, e) => this.OnTitleParametersDidChange(e);
            source.WillAppear                    += (_, e) => this.OnWillAppear(e);
            source.WillDisappear                 += (_, e) => this.OnWillDisappear(e);
        }
    }
}
