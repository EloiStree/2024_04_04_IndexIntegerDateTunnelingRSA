using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketServer
{
    public static async Task Start(string url)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();
        Console.WriteLine("Listening...");

        while (true)
        {
            HttpListenerContext listenerContext = await listener.GetContextAsync();
            if (listenerContext.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext webSocketContext = await listenerContext.AcceptWebSocketAsync(null);
                WebSocket webSocket = webSocketContext.WebSocket;

                await Task.Factory.StartNew(async () =>
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        byte[] receiveBuffer = new byte[1024];
                        WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        }
                        else
                        {
                            string message = Encoding.UTF8.GetString(receiveBuffer).TrimEnd('\0');
                            Console.WriteLine("Received: " + message);
                        }
                    }
                });
            }
            else
            {
                listenerContext.Response.StatusCode = 400;
                listenerContext.Response.Close();
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        WebSocketServer.Start("wss://81.240.94.97:3616/");
    }
}
