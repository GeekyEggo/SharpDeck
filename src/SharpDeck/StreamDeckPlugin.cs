namespace SharpDeck
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Connectivity;
    using SharpDeck.Connectivity.Net;
    using SharpDeck.Events.Received;
    using SharpDeck.Exceptions;
    using SharpDeck.Extensions;

    /// <summary>
    /// Provides a singleton wrapper for connecting and communication with a Stream Deck.
    /// </summary>
    public class StreamDeckPlugin : IStreamDeckPlugin
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// Private static field for <see cref="Current"/>.
        /// </summary>
        private static StreamDeckPlugin _current;

        /// <summary>
        /// Private member field for <see cref="Assembly"/>.
        /// </summary>
        private Assembly _assembly = Assembly.GetEntryAssembly();

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckPlugin" /> class.
        /// </summary>
        /// <param name="connectionController">The Stream Deck connection controller.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger.</param>
        internal StreamDeckPlugin(IStreamDeckConnectionController connectionController, IServiceProvider serviceProvider, ILogger<StreamDeckPlugin> logger)
            : this(connectionController)
        {
            this.Logger = logger;
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckPlugin"/> class.
        /// </summary>
        /// <param name="connectionController">The Stream Deck connection controller.</param>
        private StreamDeckPlugin(IStreamDeckConnectionController connectionController)
        {
            this.ConnectionController = connectionController;
            this.Connection.Registered += (_, __) => this.IsRegisted = true;
            this.Actions = new StreamDeckActionProvider(this.Connection);
        }

        /// <summary>
        /// Gets the singleton instance of the Stream Deck plugin.
        /// </summary>
        public static StreamDeckPlugin Current
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_current == null)
                    {
                        Initialize(new StreamDeckPlugin(new StreamDeckWebSocketConnection()));
                    }

                    return _current;
                }
            }
        }

        /// <summary>
        /// Attempts to initialize the singleton instance of the <see cref="StreamDeckPlugin"/> with the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="InvalidOperationException">Unable to set the Sound Deck Plugin singleton after it has been accessed.</exception>
        internal static void Initialize(StreamDeckPlugin instance)
        {
            lock (_syncRoot)
            {
                if (_current != null)
                {
                    throw new InvalidOperationException("Unable to set the Sound Deck Plugin singleton after it has been accessed.");
                }

                _current = instance;
            }
        }

        /// <summary>
        /// Gets the connection with the Stream Deck responsible for sending and receiving events and messages.
        /// </summary>
        public IStreamDeckConnection Connection => this.ConnectionController;

        /// <summary>
        /// Gets or sets the assembly containing the <see cref="StreamDeckAction"/> to register.
        /// </summary>
        public Assembly Assembly
        {
            get => this._assembly;
            set
            {
                if (this.IsRegisted)
                {
                    throw new InvalidOperationException("The assembly containing the actions cannot be changed after the plugin has been registered with the Stream Deck.");
                }

                this._assembly = value;
            }
        }

        /// <summary>
        /// Gets the Stream Deck actions provider.
        /// </summary>
        private StreamDeckActionProvider Actions { get; }

        /// <summary>
        /// Gets the Stream Deck connection controller.
        /// </summary>
        private IStreamDeckConnectionController ConnectionController { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger Logger { get; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is registed with the Stream Deck.
        /// </summary>
        private bool IsRegisted { get; set; } = false;

        /// <summary>
        /// Gets the service provider used to resolve new instances of the <see cref="StreamDeckAction"/>.
        /// </summary>
        private IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Runs the plugin.
        /// </summary>
        public void Run()
            => Task.WaitAll(this.RunAsync());

        /// <summary>
        /// Runs the Stream Deck plugin asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of connecting and maintaining the connection with the Stream Deck.</returns>
        public Task RunAsync(CancellationToken cancellationToken = default)
        {
            this.RegisterActions();
            return this.ConnectionController.ConnectAsync(new RegistrationParameters(Environment.GetCommandLineArgs()), cancellationToken);
        }

        /// <summary>
        /// Registers all <see cref="StreamDeckAction"/> implementations within <see cref="Assembly"/> that have the <see cref="StreamDeckActionAttribute"/>.
        /// </summary>
        private void RegisterActions()
        {
            foreach (var (type, attribute) in this.Assembly.GetTypesWithCustomAttribute<StreamDeckActionAttribute>())
            {
                if (!typeof(StreamDeckAction).IsAssignableFrom(type))
                {
                    throw new InvalidStreamDeckActionTypeException(type);
                }

                this.Actions.Register(attribute.UUID, () =>
                {
                    try
                    {
                        return (StreamDeckAction)ActivatorUtilities.CreateInstance(this.ServiceProvider, type);
                    }
                    catch (Exception ex)
                    {
                        this.Logger?.LogError(ex, $"Failed to create instance of action \"{attribute.UUID}\".");
                        throw;
                    }
                });
            }
        }
    }
}
