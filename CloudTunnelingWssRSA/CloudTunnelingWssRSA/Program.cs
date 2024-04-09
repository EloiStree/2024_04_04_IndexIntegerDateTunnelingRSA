using System;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



public class IID_WssCertificate
{

    public static string certificateFilePath = "C:\\Users\\elois\\Desktop\\Testtest\\certificate.pfx";

    // Password for the certificate file, if applicable (only for .pfx or .p12 files)
    public static string certificatePassword = "Vincent001";

    public static string uriClient = "wss://81.240.94.97:3616";
    public static string uriServer = "https://*:3616/";
}

class Program
{
    static async Task Main(string[] args)
    {

        // create a task to run on parallel thread the following code
         var task = Task.Run(async () =>
        {
            Console.WriteLine("A");
            // Load the certificate from file
            var certificate = new X509Certificate2(IID_WssCertificate.certificateFilePath, IID_WssCertificate.certificatePassword);
            var server = new WebSocketServer(certificate);
            await server.Start(IID_WssCertificate.uriServer);
        });
     
        Console.WriteLine("B");

        await Task.Delay(2000);
        Console.WriteLine("C");

        var task2 = Task.Run(async () =>
        {
            Console.WriteLine("D");

            await IID_WssClient.ConnectToWebSocketServer();
        });
     
        Console.WriteLine("E");

        // connect to the server
        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }    
        
    
}

public class IID_WssClient {


    public static async Task ConnectToWebSocketServer()
    {
        Console.WriteLine("Connecting to WebSocket server...");
        // WebSocket server endpoint
        Uri serverUri = new Uri(IID_WssCertificate.uriClient);

        // Path to your certificate file (.cer, .pfx, .p12, etc.)
      

        
        
    
        // Load the certificate from file
        X509Certificate2 certificate;
        ClientWebSocket clientWebSocket = null;
        if (!string.IsNullOrEmpty(IID_WssCertificate.certificatePassword))
        {
            certificate = new X509Certificate2(IID_WssCertificate.certificateFilePath, IID_WssCertificate.certificatePassword);
        }
        else
        {
            certificate = new X509Certificate2(IID_WssCertificate.certificateFilePath);
        }


        //Make a task tthat send random message totthe server every 5 seconds
        var task = Task.Run(async () =>
        {
            Console.WriteLine("F");
            while (true)
            {
                if (clientWebSocket == null && clientWebSocket.State== WebSocketState.Open)
                    Thread.Sleep(1);

                Console.WriteLine("FF");
                await SendMessages(clientWebSocket);
                Console.WriteLine("FFF");
                await Task.Delay(5000);
            }
        });
        // Create client WebSocket instance
        using ( clientWebSocket = new ClientWebSocket())
        {
           
            // Set the WebSocket client options
            clientWebSocket.Options.ClientCertificates.Add(certificate);

            // Connect to the WebSocket server
            await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);

            // Send and receive messages (implement your logic here)
           // await SendReceiveMessages(clientWebSocket);


            // Close the WebSocket connection
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected", CancellationToken.None);
        }

        // Dispose the certificate object when done
        certificate.Dispose();
    }
    static async Task SendMessages(ClientWebSocket clientWebSocket)
    {
        // Sending a message
        string messageToSend = "Hello, WebSocket server!";
        byte[] sendBuffer = System.Text.Encoding.UTF8.GetBytes(messageToSend);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

    }
    static async Task SendReceiveMessages(ClientWebSocket clientWebSocket)
    {
        // Sending a message
        string messageToSend = "Hello, WebSocket server!";
        byte[] sendBuffer = System.Text.Encoding.UTF8.GetBytes(messageToSend);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

        // Receiving a message
        byte[] receiveBuffer = new byte[1024];
        var receiveResult = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
        string receivedMessage = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
        Console.WriteLine($"Received message: {receivedMessage}");
    }
}


public class WebSocketServer
{
    private HttpListener _listener = new HttpListener();
    private X509Certificate2 _certificate;

    public WebSocketServer(X509Certificate2 certificate)
    {
        _certificate = certificate;
    }

    public async Task Start(string prefix)
    {
        _listener.Prefixes.Add(prefix);
        _listener.Start();
        Console.WriteLine("Listening for WebSocket connections...");

        while (true)
        {
            var context = await _listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                await ProcessWebSocketRequest(context);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private async Task ProcessWebSocketRequest(HttpListenerContext context)
    {
        var webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
        var socket = webSocketContext.WebSocket;

        while (socket.State == WebSocketState.Open)
        {
            // Receive messages from the client
            var buffer = new byte[1024];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
            else
            {
                // Handle received message
                var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message: {message}");

                // Echo the message back to the client
                await socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
