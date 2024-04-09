using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NetCoreServer;


using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using NetCoreServer;
using WssChatClient;

namespace WssChatServer
{
    class ChatSession : WssSession
    {
        public ChatSession(WssServer server) : base(server) { }

        public override void OnWsConnected(HttpRequest request)
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} connected!");

            // Send invite message
            string message = "Hello from WebSocket chat! Please send a message or '!' to disconnect the client!";
            SendTextAsync(message);
        }

        public override void OnWsDisconnected()
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} disconnected!");
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            // Multicast message to all connected sessions
            ((WssServer)Server).MulticastText(message);

            // If the buffer starts with '!' the disconnect the current session
            if (message == "!")
                Close();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket session caught an error with code {error}");
        }
    }

    class ChatServer : WssServer
    {
        public ChatServer(SslContext context, IPAddress address, int port) : base(context, address, port) { }

        protected override SslSession CreateSession() { return new ChatSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket server caught an error with code {error}");
        }
    }

    class ProgramBB
    {
        static void MainCD(string[] args)
        {
            // WebSocket server port
            int port = 3616;
            // WebSocket server content path
            string www = "www/wss";
            if (!Directory.Exists(www))
                Directory.CreateDirectory(www);


            if (args.Length > 1)
                www = args[1];

            Console.WriteLine($"WebSocket server port: {port}");
            Console.WriteLine($"WebSocket server static content path: {www}");
            Console.WriteLine($"WebSocket server website: https://localhost:{port}/chat/index.html");

            Console.WriteLine();

            // Create and prepare a new SSL server context
            var context = new SslContext(SslProtocols.Tls13, new X509Certificate2(IID_WssCertificate.certificateFilePath, IID_WssCertificate.certificatePassword));

            // Create a new WebSocket server
            var server = new ChatServer(context, IPAddress.Any, port);
            server.AddStaticContent(www, "/");

            // Start the server
            Console.Write("Server starting...");
            server.Start();
            Console.WriteLine("Done!");

            //Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            //// Perform text input
            //for (; ; )
            //{
            //    string line = Console.ReadLine();
            //    if (string.IsNullOrEmpty(line))
            //        break;

            //    // Restart the server
            //    if (line == "!")
            //    {
            //        Console.Write("Server restarting...");
            //        server.Restart();
            //        Console.WriteLine("Done!");
            //    }

            //    // Multicast admin message to all sessions
            //    line = "(admin) " + line;
            //    server.MulticastText(line);
            //}


            // WebSocket server address
            string address = "127.0.0.1";

            // WebSocket server port
            int portclient = 3616;
         

            Console.WriteLine($"WebSocket server address: {address}");
            Console.WriteLine($"WebSocket server port: {portclient}");

            Console.WriteLine();

         
            // Create a new TCP chat client
            var client = new ChatClient(context, address, portclient);

            // Connect the client
            Console.Write("Client connecting...");
            client.ConnectAsync();
            Console.WriteLine("Done!");

            Console.WriteLine("Press Enter to stop the client or '!' to reconnect the client...");

            // Perform text input
            for (; ; )
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Reconnect the client
                if (line == "!")
                {
                    Console.Write("Client reconnecting...");
                    if (client.IsConnected)
                        client.ReconnectAsync();
                    else
                        client.ConnectAsync();
                    Console.WriteLine("Done!");
                    continue;
                }

                // Send the entered text to the chat server
                client.SendTextAsync(line);
            }

            // Disconnect the client
            Console.Write("Client disconnecting...");
            client.DisconnectAndStop();
            Console.WriteLine("Done!");

            // Stop the server
            Console.Write("Server stopping...");
            server.Stop();
            Console.WriteLine("Done!");
        }
    }
}
