using WebSocketSharp;
using WebSocketSharp.Server;

namespace Wss.Console
{
    /// <summary>
    /// Provides a light-weight web socket server.
    /// </summary>
    public class Program
    {
        private const string MOCK_KEY_UP = "{\"action\": \"com.elgato.example.action1\",\"event\": \"keyUp\",\"context\": null,\"device\": null,\"payload\": {\"settings\": null,\"coordinates\": {\"column\": 3, \"row\": 1},\"state\": 0,\"userDesiredState\": 1,\"isInMultiAction\": false}}";
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
            System.Console.WriteLine($"Server started at: {url}");

            while (true)
            {
                System.Console.Write("Send: ");
                var input = System.Console.ReadLine();
                if (input == "x" || input == "exit")
                {
                    break;
                }
                else if (input == "up")
                {
                    System.Console.WriteLine($"Sent: {MOCK_KEY_UP}");
                    wssv.WebSocketServices.Broadcast(MOCK_KEY_UP);
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
            {
                var msg = $"Received: {e.Data}";

                System.Console.WriteLine(msg);
                this.Send(msg);
            }
        }
    }
}
