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
        public static ListenAsLocalWebsocket Instance = new ListenAsLocalWebsocket();

        public static HttpListener httpListener;

        public static List<HttpListenerWebSocketContext> m_connectedCallback = new List<HttpListenerWebSocketContext>();

        public static async Task Start(string httpListenerPrefix = "http://localhost:5002/")
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(httpListenerPrefix);
            

            httpListener.Start();


            Console.WriteLine("\n\n\n\n");
            ServerConsole.WriteLine("Local Websocket: "+ httpListenerPrefix);


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

        private static async void ProcessWebSocketRequest(HttpListenerContext context)
        {
            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
            m_connectedCallback.Add(webSocketContext);
            WebSocket webSocket = webSocketContext.WebSocket;
            ServerConsole.WriteLine($"Reqiest URI {webSocketContext.RequestUri}");


            int indexLockedOn = 0;
            bool isIndexLocked = false;


            byte[] buffer = new byte[600];

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {

                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    ServerConsole.WriteLine($"Websocket, Received message type: {result.MessageType} {result.Count} {DateTime.UtcNow}");

                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        byte[]  receivedMessageBytes = new byte[16];
                        Array.Copy(buffer, 0, receivedMessageBytes, 0,16);
                        WebSocketClientToServerRSA.SentToServerAsBinary(receivedMessageBytes);
                        continue;
                    }
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        WebSocketClientToServerRSA.SentToServerAsUTF8(receivedMessage);
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

        public void BroadcastToConnected(string message)
        {


            byte[] buffer = Encoding.UTF8.GetBytes(message);
            BroadcastToConnected(buffer, WebSocketMessageType.Text);
        }
        public void BroadcastToConnected(byte[] message)
        {
            BroadcastToConnected(message, WebSocketMessageType.Binary);
        }
        public void BroadcastToConnected(byte[] message, WebSocketMessageType typeSend)
        {

            for (int i = m_connectedCallback.Count - 1; i >= 0; i--)
            {
                try
                {
                    WebSocket webSocket = m_connectedCallback[i].WebSocket;
                    if (webSocket.State == WebSocketState.Open)
                    {
                        webSocket.SendAsync(new ArraySegment<byte>(message), typeSend, true, CancellationToken.None);
                    }
                    else
                    {
                        m_connectedCallback.RemoveAt(i);
                    }
                }
                catch (Exception e)
                {
                    ServerConsole.WriteLine($"Error: {e.Message}");
                }
            }
        }
    }
}