namespace SharpDeck.Plugin
{
    using System.Diagnostics;

    /// <summary>
    /// A basic plugin utilising the SharpDeck library.
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
            Debugger.Launch();
#endif

            using (var client = new StreamDeckClient(args))
            {
                // register our custom action, and start the client
                client.RegisterAction<CounterAction>("com.sharpdeck.testplugin.counter");
                client.Start();
            }
        }
    }
}
