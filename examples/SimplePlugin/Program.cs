namespace SimplePlugin
{
    using System.Diagnostics;
    using SharpDeck;

    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif

            StreamDeckPlugin.Run();
        }
    }
}
