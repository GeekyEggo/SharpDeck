namespace SharedCounter
{
    using System.Diagnostics;
    using SharpDeck;

    /// <summary>
    /// The plug-in.
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

            StreamDeckPlugin.Run();
        }
    }
}
