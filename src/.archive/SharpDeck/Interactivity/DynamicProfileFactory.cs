namespace SharpDeck.Interactivity
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Connectivity;
    using SharpDeck.Enums;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides a factory for creating <see cref="DynamicProfile{T}"/>.
    /// </summary>
    internal class DynamicProfileFactory : IDynamicProfileFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProfileFactory" /> class.
        /// </summary>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="serviceProvider">The service provider responsible for creating new instances of <see cref="IDynamicProfileController{TItem}" />.</param>
        /// <param name="loggerFactory">The optional logger factory.</param>
        public DynamicProfileFactory(IStreamDeckConnection connection, RegistrationParameters registrationParameters, IServiceProvider serviceProvider, ILoggerFactory loggerFactory = null)
        {
            this.Connection = connection;
            this.LoggerFactory = loggerFactory;
            this.RegistrationParameters = registrationParameters;
            this.ServiceProvider = serviceProvider;

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
        /// Gets the connection with the Stream Deck.
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
        /// Gets the service provider responsible for creating new instances of <see cref="IDynamicProfileController{TItem}" />.
        /// </summary>
        private IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Creates a new <see cref="DynamicProfile{TItem}"/>.
        /// </summary>
        /// <typeparam name="TController">The type of the dynamic profile controller.</typeparam>
        /// <typeparam name="TItem">The type of the items the manager is capable of handling.</typeparam>
        /// <param name="deviceUUID">The device UUID the dynamic profile is for.</param>
        /// <returns>The dynamic profile.</returns>
        public IDynamicProfile<TItem> Create<TController, TItem>(string deviceUUID)
            where TController : class, IDynamicProfileController<TItem>
        {
            if (!this.Devices.TryGetValue(deviceUUID, out var device))
            {
                throw new ArgumentException($"A device with UUID \"{deviceUUID}\" could not be found.");
            }

            var controller = ActivatorUtilities.CreateInstance<TController>(this.ServiceProvider);
            if (device.Type == DeviceType.CorsairGKeys
                || !controller.TryGetProfileName(device.Type, out var profile))
            {
                throw new NotSupportedException($"Cannot show dynamic profile on device \"{deviceUUID}\" as \"{device.Type}\" is not a supported device type.");
            }

            var ctx = new DynamicProfileContext<TItem>(this.Connection, this.RegistrationParameters.PluginUUID, device, profile);
            return new DynamicProfile<TItem>(ctx, controller, this.LoggerFactory?.CreateLogger<DynamicProfile<TItem>>());
        }
    }
}
