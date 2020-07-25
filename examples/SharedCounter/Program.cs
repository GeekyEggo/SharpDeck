namespace SharedCounter
{
    using System.Threading.Tasks;
    using SharpDeck;

    /// <summary>
    /// The plugin.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif

            StreamDeckPlugin.Create()
                .OnRegistered(conn => _ = SetInitialCountAsync(conn))
                .Run();
        }

        /// <summary>
        /// Sets the initial count.
        /// </summary>
        /// <param name="conn">The Stream Deck connection.</param>
        private static Task SetInitialCountAsync(IStreamDeckConnection conn)
        {
            return Task.Run(async () =>
            {
                var settings = await conn.GetGlobalSettingsAsync<GlobalSettings>();
                Count.Instance.Set(settings.Count);
            });
        }
    }
}
