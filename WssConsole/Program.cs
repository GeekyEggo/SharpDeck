using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WssConsole
{
    /// <summary>
    /// Provides a light-weight web socket server.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            const string url = "ws://localhost:23655";

            var wssv = new WebSocketServer(url);
            wssv.AddWebSocketService<MainService>("/");

            wssv.Start();
            Console.WriteLine($"Server started at: {url}");

            while (true)
            {
                Console.Write("Send: ");
                var input = Console.ReadLine();
                if (input == "x" || input == "exit")
                {
                    break;
                }
                else
                {
                    wssv.WebSocketServices.Broadcast(input);
                }
            }

            wssv.Stop();
        }

        /// <summary>
        /// A basic service that confirms when a message is received.
        /// </summary>
        public class MainService : WebSocketBehavior
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MainService"/> class.
            /// </summary>
            public MainService()
            {
                this.IgnoreExtensions = true;
            }

            /// <summary>
            /// Called when the <see cref="T:WebSocketSharp.WebSocket" /> used in a session receives a message.
            /// </summary>
            /// <param name="e">A <see cref="T:WebSocketSharp.MessageEventArgs" /> that represents the event data passed to
            /// a <see cref="E:WebSocketSharp.WebSocket.OnMessage" /> event.</param>
            protected override void OnMessage(MessageEventArgs e)
                => this.Send($"CONFIRM: [{e.Data}]");
        }
    }
}
