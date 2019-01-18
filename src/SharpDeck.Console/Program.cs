namespace SharpDeck.Console
{
    using Models;
    using SharpDeck.Enums;
    using System;

    /// <summary>
    /// Provides a light-weight console that demonstrates the <see cref="StreamDeckClient"/>.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var regParams = new RegistrationParameters
            {
                Port = 23655
            };

            using (var client = new StreamDeckClient(regParams))
            {
                client.KeyUp += (_, e) =>
                {
                    client.SetTitleAsync("context", "New title here", TargetType.Both);
                };

                client.Start();
                Console.WriteLine("Plugin started, press [x] to exit");

                var input = new ConsoleKeyInfo();
                while (input.Key != ConsoleKey.X)
                {
                    input = Console.ReadKey();
                }

                client.Stop();

                Console.ReadKey();
            }
        }
    }
}
