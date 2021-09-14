namespace SharpDeck.Interactivity
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using SharpDeck.Connectivity;
    using SharpDeck.DependencyInjection;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides a factory for creating <see cref="DrillDown{TManager, TItem}"/>.
    /// </summary>
    internal class DrillDownFactory : IDrillDownFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrillDownFactory" /> class.
        /// </summary>
        /// <param name="connection">The connection to the Stream Deck.</param>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="activator">The the activator responsible for creating new instances of <see cref="IDrillDownManager{TItem}"/>.</param>
        public DrillDownFactory(IStreamDeckConnection connection, RegistrationParameters registrationParameters, IActivator activator)
        {
            this.Activator = activator;
            this.Connection = connection;
            this.RegistrationParameters = registrationParameters;

            this.Connection.DeviceDidConnect += (_, args) => this.Devices.TryAdd(
                args.Device,
                new IdentifiableDeviceInfo
                {
                    Id = args.Device,
                    Name = args.DeviceInfo.Name,
                    Size = args.DeviceInfo.Size,
                    Type = args.DeviceInfo.Type
                });

            foreach (var device in registrationParameters.Info.Devices)
            {
                this.Devices.TryAdd(device.Id, device);
            }
        }

        /// <summary>
        /// Gets the activator responsible for creating new instances of <see cref="IDrillDownManager{TItem}"/>.
        /// </summary>
        private IActivator Activator { get; }

        /// <summary>
        /// Gets the connection to the Stream Deck.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the devices.
        /// </summary>
        private ConcurrentDictionary<string, IdentifiableDeviceInfo> Devices { get; } = new ConcurrentDictionary<string, IdentifiableDeviceInfo>();

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; }

        /// <summary>
        /// Creates a new <see cref="DrillDown{TManager, TItem}"/>.
        /// </summary>
        /// <typeparam name="TManager">The type of the drill-down manager.</typeparam>
        /// <typeparam name="TItem">The type of the items the manager is capable of handling.</typeparam>
        /// <param name="deviceUUID">The device UUID the drill-down is for.</param>
        /// <returns>The drill-down.</returns>
        public IDrillDown<TManager, TItem> Create<TManager, TItem>(string deviceUUID)
            where TManager : class, IDrillDownManager<TItem>
        {
            if (!this.Devices.TryGetValue(deviceUUID, out var device))
            {
                throw new ArgumentException($"A device with UUID \"{deviceUUID}\" could not be found.");
            }

            var manager = (TManager)this.Activator.CreateInstance(typeof(TManager));
            if (device.Type == Enums.DeviceType.CorsairGKeys
                || !manager.SupportedDevices.Contains(device.Type))
            {
                throw new NotSupportedException($"Cannot show drill-down on device \"{deviceUUID}\" as \"{device.Type}\" is not a supported device type.");
            }

            return new DrillDown<TManager, TItem>(this.Connection, this.RegistrationParameters.PluginUUID, device, manager);
        }
    }
}
