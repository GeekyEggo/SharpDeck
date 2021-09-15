namespace SharpDeck.Interactivity
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Connectivity;
    using SharpDeck.DependencyInjection;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides a factory for creating <see cref="DrillDown{T}"/>.
    /// </summary>
    internal class DrillDownFactory : IDrillDownFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrillDownFactory" /> class.
        /// </summary>
        /// <param name="connection">The connection to the Stream Deck.</param>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="activator">The the activator responsible for creating new instances of <see cref="IDrillDownController{TItem}" />.</param>
        /// <param name="loggerFactory">The optional logger factory.</param>
        public DrillDownFactory(IStreamDeckConnection connection, RegistrationParameters registrationParameters, IActivator activator, ILoggerFactory loggerFactory = null)
        {
            this.Activator = activator;
            this.Connection = connection;
            this.LoggerFactory = loggerFactory;
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
        /// Gets the activator responsible for creating new instances of <see cref="IDrillDownController{TItem}"/>.
        /// </summary>
        private IActivator Activator { get; }

        /// <summary>
        /// Gets the connection to the Stream Deck.
        /// </summary>
        private IStreamDeckConnection Connection { get; }

        /// <summary>
        /// Gets the devices.
        /// </summary>
        private ConcurrentDictionary<string, IDevice> Devices { get; } = new ConcurrentDictionary<string, IDevice>();

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        private ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; }

        /// <summary>
        /// Creates a new <see cref="DrillDown{TItem}"/>.
        /// </summary>
        /// <typeparam name="TController">The type of the drill down controller.</typeparam>
        /// <typeparam name="TItem">The type of the items the manager is capable of handling.</typeparam>
        /// <param name="deviceUUID">The device UUID the drill down is for.</param>
        /// <returns>The drill down.</returns>
        public IDrillDown<TItem> Create<TController, TItem>(string deviceUUID)
            where TController : class, IDrillDownController<TItem>
        {
            if (!this.Devices.TryGetValue(deviceUUID, out var device))
            {
                throw new ArgumentException($"A device with UUID \"{deviceUUID}\" could not be found.");
            }

            var controller = (TController)this.Activator.CreateInstance(typeof(TController));
            if (device.Type == Enums.DeviceType.CorsairGKeys
                || !controller.SupportedDevices.Contains(device.Type))
            {
                throw new NotSupportedException($"Cannot show drill down on device \"{deviceUUID}\" as \"{device.Type}\" is not a supported device type.");
            }

            var ctx = new DrillDownContext<TItem>(this.Connection, this.RegistrationParameters.PluginUUID, device);
            return new DrillDown<TItem>(ctx, controller, this.LoggerFactory?.CreateLogger<DrillDown<TItem>>());
        }
    }
}
