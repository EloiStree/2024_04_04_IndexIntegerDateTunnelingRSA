using System.Net;
using System.Net.WebSockets;
using System.Runtime.ConstrainedExecution;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

public class IDD_WebSocketServer
{
    private HttpListener m_listener = new HttpListener();
    private X509Certificate2 m_certificate;

    public IDD_WebSocketServer(X509Certificate2 certificate)
    {
        m_certificate = certificate;
        X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadWrite);
        if (!store.Certificates.Contains(m_certificate))
        {
            store.Add(m_certificate);
        }
        store.Close();

    }

    public async Task Start(string prefix)
    {
        m_listener = new HttpListener();
        m_listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//SslProtocols.Tls12;

        //m_listener.Prefixes.Add(prefix);
        m_listener.Prefixes.Add("https://localhost:3616/");
        //m_listener.Prefixes.Add("https://127.0.01:3616/");

        // m_listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;


        //_listener.AuthenticationSchemes = AuthenticationSchemes.None;


        //Set the listener with the _certificate





        m_listener.Start();
        Console.WriteLine("Listening for WebSocket connections...");
       



        while (true)
        {
            Console.WriteLine("J...");
            var context = await m_listener.GetContextAsync();
            Console.WriteLine("JJ...");
            if (context.Request.IsWebSocketRequest)
            {
                Console.WriteLine("JJJ...");
                await HandleWebSocketRequest(context);
            }
            else
            {
                // Handle HTTP request
                await HandleHttpRequest(context);
            }
        }
        Console.WriteLine("Listening for WebSocket close...");
    }


    static async Task HandleHttpRequest(HttpListenerContext context)
    {
        // Handle HTTP request
        string responseString = "<html><body>Hello from HTTP!</body></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

        context.Response.ContentType = "text/html";
        context.Response.ContentLength64 = buffer.Length;

        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        context.Response.Close();
    }

    static async Task HandleWebSocketRequest(HttpListenerContext context)
    {
        // Accept WebSocket connection
        HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);

        // Handle WebSocket connection
        WebSocket websocket = wsContext.WebSocket;

        byte[] receiveBuffer = new byte[1024];
        while (websocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await websocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                // Echo received message
                await websocket.SendAsync(new ArraySegment<byte>(receiveBuffer, 0, result.Count), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }
    }
}
