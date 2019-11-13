namespace SharpDeck
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SharpDeck.DependencyInjection;
    using SharpDeck.Exceptions;
    using SharpDeck.Extensions;
    using SharpDeck.Manifest;

    /// <summary>
    /// Provides a wrapper for running a <see cref="StreamDeckClient"/>, after auto-registering all actions that implement <see cref="StreamDeckAction"/> and have the attribute <see cref="StreamDeckActionAttribute"/>.
    /// </summary>
    public static class StreamDeckPlugin
    {
        /// <summary>
        /// The maximum retry count.
        /// </summary>
        private const int MAX_RETRY_COUNT = 3;

        /// <summary>
        /// Starts the <see cref="StreamDeckClient"/>.
        /// </summary>
        /// <param name="args">The optional command line arguments supplied when running the plug-in; when null, <see cref="Environment.GetCommandLineArgs"/> is used.</param>
        /// <param name="assembly">The optional assembly containing the <see cref="StreamDeckAction"/>; when null, <see cref="Assembly.GetEntryAssembly"/> is used.</param>
        /// <param name="provider">The optional service provider to resolve new instances of the registered <see cref="StreamDeckAction"/>.</param>
        /// <param name="logger">The optional logger.</param>
        /// <param name="setup">The optional additional setup.</param>
        public static async void Run(string[] args = null, Assembly assembly = null, IServiceProvider provider = null, ILogger logger = null, Action<IStreamDeckClient> setup = null)
            => await RunAsync(args, assembly ?? Assembly.GetCallingAssembly(), provider, logger, setup).ConfigureAwait(false);

        /// <summary>
        /// Starts the <see cref="StreamDeckClient"/> asynchronously.
        /// </summary>
        /// <param name="args">The optional command line arguments supplied when running the plug-in; when null, <see cref="Environment.GetCommandLineArgs"/> is used.</param>
        /// <param name="assembly">The optional assembly containing the <see cref="StreamDeckAction"/>; when null, <see cref="Assembly.GetEntryAssembly"/> is used.</param>
        /// <param name="provider">The optional service provider to resolve new instances of the registered <see cref="StreamDeckAction"/>.</param>
        /// <param name="logger">The optional logger.</param>
        /// <param name="setup">The optional additional setup.</param>
        /// <returns>The task of running the client.</returns>
        public static Task RunAsync(string[] args = null, Assembly assembly = null, IServiceProvider provider = null, ILogger logger = null, Action<IStreamDeckClient> setup = null)
            => RunAsync(new StreamDeckPluginInfo(assembly ?? Assembly.GetCallingAssembly(), args, provider, logger, setup));

        /// <summary>
        /// Runs the <see cref="StreamDeckClient" /> asynchronously.
        /// </summary>
        /// <param name="info">The information used to determine how the client should be setup and started.</param>
        /// <param name="retryCount">The retry count used when attempting to restart a failed client.</param>
        private static async Task RunAsync(StreamDeckPluginInfo info, int retryCount = 0)
        {
            try
            {
                using (var client = new StreamDeckClient(info.RegistrationParameters, info.Logger))
                {
                    RegisterActions(client, info.Assembly, info.Provider);
                    info.Setup(client);

                    await client.StartAsync(CancellationToken.None);
                }
            }
            catch (Exception ex) when (!(ex is InvalidStreamDeckActionTypeException) && retryCount < MAX_RETRY_COUNT)
            {
                await RunAsync(info, retryCount + 1);
            }
        }

        /// <summary>
        /// Registers all <see cref="StreamDeckAction"/> implementations within <see cref="Assembly"/> that have the <see cref="StreamDeckActionAttribute"/>.
        /// </summary>
        /// <param name="client">The Stream Deck client.</param>
        /// <param name="assembly">The assembly containing the actions.</param>
        /// <param name="provider">The service provider used to resolve new instances of <see cref="StreamDeckAction"/>.</param>
        private static void RegisterActions(StreamDeckClient client, Assembly assembly, IServiceProvider provider)
        {
            foreach (var (type, attribute) in assembly.GetTypesWithCustomAttribute<StreamDeckActionAttribute>())
            {
                if (!typeof(StreamDeckAction).IsAssignableFrom(type))
                {
                    throw new InvalidStreamDeckActionTypeException(type);
                }

                client.RegisterAction(attribute.UUID, () => (StreamDeckAction)ActivatorUtilities.CreateInstance(provider, type));
            }
        }
    }
}
