namespace SharpDeck.DependencyInjection
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Events.Received;

    /// <summary>
    /// Provides information to be used when starting a <see cref="StreamDeckClient"/> with auto registration.
    /// </summary>
    internal class StreamDeckPluginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckPluginInfo"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="setup">The optional additional setup.</param>
        /// <param name="logger">The logger.</param>
        public StreamDeckPluginInfo(Assembly assembly = null, string[] args = null, IServiceProvider provider = null, ILogger logger = null, Action<IStreamDeckClient> setup = null)
        {
            this.Assembly = assembly;
            this.Logger = logger;
            this.Provider = provider ?? new ServiceCollection().BuildServiceProvider();
            this.RegistrationParameters = RegistrationParameters.Parse(args);
            this.Setup = setup ?? (_ => { });
        }

        /// <summary>
        /// Gets the assembly containing the <see cref="StreamDeckAction"/> to register.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the provider used to resolve new instances of the <see cref="StreamDeckAction"/>.
        /// </summary>
        public IServiceProvider Provider { get; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        public RegistrationParameters RegistrationParameters { get; }

        /// <summary>
        /// Gets the delegate that provides additional setup.
        /// </summary>
        public Action<IStreamDeckClient> Setup { get; }
    }
}
