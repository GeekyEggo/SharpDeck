namespace SharpDeck.Console
{
    using Models;
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
                client.KeyUp += Client_KeyUp;

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

        /// <summary>
        /// Handles the <see cref="StreamDeckClient.KeyUp"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Events.KeyActionEventArgs"/> instance containing the event data.</param>
        private static void Client_KeyUp(object sender, Events.ActionEventArgs<KeyPayload> e)
            => System.Console.WriteLine("KEY UP TRIGGERED");
    }
}
