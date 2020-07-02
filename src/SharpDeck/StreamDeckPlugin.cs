namespace SharpDeck
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using SharpDeck.Connectivity;
    using SharpDeck.Connectivity.Net;
    using SharpDeck.Events.Received;
    using SharpDeck.Exceptions;
    using SharpDeck.Extensions;
    using SharpDeck.Manifest;

    /// <summary>
    /// Provides a wrapper for connecting and communication with a Stream Deck.
    /// </summary>
    public class StreamDeckPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckPlugin"/> class.
        /// </summary>
        /// <param name="args">The optional arguments as supplied by the Elgato Stream Deck; when null, <see cref="Environment.GetCommandLineArgs"/> is used.</param>
        /// <param name="assembly">The optional assembly containing the <see cref="StreamDeckAction"/> to be registered.</param>
        private StreamDeckPlugin(string[] args = null, Assembly assembly = null)
        {
            this.RegistrationParameters = new RegistrationParameters(args);
            this.Connection = new StreamDeckWebSocketConnection();
            this.Connection.Registered += (_, __) => this.Registered(this.Connection);
            this.Actions = new StreamDeckActionProvider(this.Connection);

            // configurable settings
            this.Assembly = assembly;
            this.Registered = _ => { };
            this.Setup = _ => { };
            this.ServiceProvider = new ServiceCollection().BuildServiceProvider();
        }

        /// <summary>
        /// Gets or sets the assembly containing the <see cref="StreamDeckAction"/> to register.
        /// </summary>
        private Assembly Assembly { get; set; }

        /// <summary>
        /// Gets the connection with the Stream Deck responsible for sending and receiving events and messages.
        /// </summary>
        private StreamDeckWebSocketConnection Connection { get; }

        /// <summary>
        /// Gets the Stream Deck actions provider.
        /// </summary>
        private StreamDeckActionProvider Actions { get; }

        /// <summary>
        /// Gets or sets the delegate that is invoked when <see cref="IStreamDeckConnection.Registered"/> occurs.
        /// </summary>
        private Action<IStreamDeckConnection> Registered { get; set; }

        /// <summary>
        /// Gets or sets the delegate that provides additional setup.
        /// </summary>
        private Action<IStreamDeckConnection> Setup { get; set; }

        /// <summary>
        /// Gets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; }

        /// <summary>
        /// Gets the provider used to resolve new instances of the <see cref="StreamDeckAction"/>.
        /// </summary>
        private IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Creates a new instance of a <see cref="StreamDeckPlugin"/>.
        /// </summary>
        /// <param name="args">The optional arguments as supplied by the Elgato Stream Deck; when null, <see cref="Environment.GetCommandLineArgs"/> is used.</param>
        /// <param name="assembly">The optional assembly containing the <see cref="StreamDeckAction"/> to be registered; when null, <see cref="Assembly.GetCallingAssembly"/> is used.</param>
        /// <returns>The Stream Deck plugin.</returns>
        public static StreamDeckPlugin Create(string[] args = null, Assembly assembly = null)
            => new StreamDeckPlugin(args, assembly ?? Assembly.GetCallingAssembly());

        /// <summary>
        /// Runs the plugin.
        /// </summary>
        /// <param name="args">The optional arguments as supplied by the Elgato Stream Deck; when null, <see cref="Environment.GetCommandLineArgs"/> is used.</param>
        public static void Run(string[] args = null)
            => new StreamDeckPlugin(args, Assembly.GetCallingAssembly()).Run();

        /// <summary>
        /// Runs the plugin asynchronously.
        /// </summary>
        /// <param name="args">The optional arguments as supplied by the Elgato Stream Deck; when null, <see cref="Environment.GetCommandLineArgs"/> is used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task of running the plugin.</returns>
        public static Task RunAsync(CancellationToken cancellationToken, string[] args = null)
            => new StreamDeckPlugin(args, Assembly.GetCallingAssembly()).RunAsync(cancellationToken);

        /// <summary>
        /// Sets the delegate to applied when the plugin is registered with the Stream Deck.
        /// </summary>
        /// <param name="action">The delegate.</param>
        /// <returns>This instance.</returns>
        public StreamDeckPlugin OnRegistered(Action<IStreamDeckConnection> action)
        {
            this.Registered = action;
            return this;
        }

        /// <summary>
        /// Sets the delegate to applied immediately before a connection is established with the Stream Deck.
        /// </summary>
        /// <param name="action">The delegate.</param>
        /// <returns>This instance.</returns>
        public StreamDeckPlugin OnSetup(Action<IStreamDeckConnection> action)
        {
            this.Setup = action;
            return this;
        }

        /// <summary>
        /// Runs the plugin.
        /// </summary>
        public void Run()
            => Task.WaitAll(this.RunAsync(CancellationToken.None));

        /// <summary>
        /// Runs the Stream Deck plugin asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of connecting and maintaining the connection with the Stream Deck.</returns>
        public Task RunAsync(CancellationToken cancellationToken)
        {
            this.RegisterActions();
            this.Setup(this.Connection);

            return this.Connection.ConnectAsync(this.RegistrationParameters, cancellationToken);
        }

        /// <summary>
        /// Sets the <see cref="IServiceProvider"/> to be used when resolving instances of <see cref="StreamDeckAction"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>This instance.</returns>
        public StreamDeckPlugin WithServiceProvider(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
            return this;
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

                this.Actions.Register(attribute.UUID, () => (StreamDeckAction)ActivatorUtilities.CreateInstance(this.ServiceProvider, type));
            }
        }
    }
}
