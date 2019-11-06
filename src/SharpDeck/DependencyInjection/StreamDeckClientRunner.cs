namespace SharpDeck.DependencyInjection
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using SharpDeck.Exceptions;
    using SharpDeck.Extensions;
    using SharpDeck.Manifest;

    /// <summary>
    /// Provides helper methods for starting a <see cref="StreamDeckClient"/> that automatically registers <see cref="StreamDeckAction"/>.
    /// </summary>
    internal static class StreamDeckClientRunner
    {
        /// <summary>
        /// The maximum retry count.
        /// </summary>
        private const int MAX_RETRY_COUNT = 3;

        /// <summary>
        /// Runs the <see cref="StreamDeckClient" /> asynchronously.
        /// </summary>
        /// <param name="info">The information used to determine how the client should be setup and started.</param>
        /// <param name="retryCount">The retry count used when attempting to restart a failed client.</param>
        public static async Task RunAsync(StreamDeckClientRunnerInfo info, int retryCount = 0)
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
