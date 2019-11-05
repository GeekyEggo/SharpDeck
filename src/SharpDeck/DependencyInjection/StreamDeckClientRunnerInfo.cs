namespace SharpDeck.DependencyInjection
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Events.Received;
    using SharpDeck.Exceptions;

    /// <summary>
    /// Provides information to be used when starting a <see cref="StreamDeckClient"/> with auto registration.
    /// </summary>
    internal class StreamDeckClientRunnerInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckClientRunnerInfo"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="errorHandler">The error handler.</param>
        /// <param name="logger">The logger.</param>
        public StreamDeckClientRunnerInfo(string[] args = null, Assembly assembly = null, IServiceProvider provider = null, EventHandler<StreamDeckConnectionErrorEventArgs> errorHandler = null, ILogger logger = null)
        {
            this.Assembly = assembly ?? Assembly.GetEntryAssembly();
            this.Provider = provider ?? new ServiceCollection().BuildServiceProvider();
            this.ErrorHandler = errorHandler;
            this.Logger = logger;
            this.RegistrationParameters = RegistrationParameters.Parse(args ?? Environment.GetCommandLineArgs());
        }

        /// <summary>
        /// Gets the assembly containing the <see cref="StreamDeckAction"/> to register.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Gets the provider used to resolve new instances of the <see cref="StreamDeckAction"/>.
        /// </summary>
        public IServiceProvider Provider { get; }

        /// <summary>
        /// Gets the error handler.
        /// </summary>
        public EventHandler<StreamDeckConnectionErrorEventArgs> ErrorHandler { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        public RegistrationParameters RegistrationParameters { get; }
    }
}
