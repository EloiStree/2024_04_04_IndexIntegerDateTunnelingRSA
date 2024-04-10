using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CloudTunnelingLocalGateRSA
{

    internal class ListenAsLocalWebsocket
    {
        public ListenAsLocalWebsocket Instance = new ListenAsLocalWebsocket();

        public HttpListener httpListener;

        public async Task Start(string httpListenerPrefix = "http://localhost:5002")
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(httpListenerPrefix);
            httpListener.Start();

            ServerConsole.WriteLine("WebSocket server is running...");


            while (true)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    ProcessWebSocketRequest(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async void ProcessWebSocketRequest(HttpListenerContext context)
        {
            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
            WebSocket webSocket = webSocketContext.WebSocket;
            ServerConsole.WriteLine($"Reqiest URI {webSocketContext.RequestUri}");


            int indexLockedOn = 0;
            bool isIndexLocked = false;


            byte[] buffer = new byte[600];
            byte[] receivedMessageBytes = new byte[16];

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {

                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    ServerConsole.WriteLine($"Received message type: {result.MessageType} {result.Count}");

                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        receivedMessageBytes = new byte[16];
                        Array.Copy(buffer, 0, receivedMessageBytes, 0,16);
                        WebSocketClientToServerRSA.SentToServerAsBinary(receivedMessageBytes);
                        continue;
                    }
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        WebSocketClientToServerRSA.SentToServerAsUTF8(receivedMessage);
                        ServerConsole.WriteLine($"Received message : {receivedMessage}");
                        continue;

                    }
                }
                catch (Exception e)
                {
                    ServerConsole.WriteLine($"Error: {e.Message}");
                }
                finally
                {
                    if (isIndexLocked)
                    {
                        ServerConsole.WriteLine($"Index {indexLockedOn} is locked");
                    }
                }
            }



        }
    }
}