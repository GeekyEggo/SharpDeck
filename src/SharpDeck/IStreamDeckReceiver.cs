﻿namespace SharpDeck
{
    using Events;
    using System;

    /// <summary>
    /// Provides events that are received from an Elgato Stream Deck.
    /// </summary>
    public interface IStreamDeckReceiver : IStreamDeckActionReceiver
    {
        /// <summary>
        /// Occurs when a monitored application is launched.
        /// </summary>
        event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidLaunch;

        /// <summary>
        /// Occurs when a monitored application is terminated.
        /// </summary>
        event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidTerminate;

        /// <summary>
        /// Occurs when a device is plugged to the computer.
        /// </summary>
        event EventHandler<DeviceConnectEventArgs> DeviceDidConnect;

        /// <summary>
        /// Occurs when a device is unplugged from the computer.
        /// </summary>
        event EventHandler<DeviceEventArgs> DeviceDidDisconnect;

        /// <summary>
        /// Occurs when <see cref="IStreamDeckSender.GetGlobalSettingsAsync(string)"/> has been called to retrieve the persistent global data stored for the plugin.
        /// </summary>
        event EventHandler<StreamDeckEventArgs<SettingsPayload>> DidReceiveGlobalSettings;
    }
}
