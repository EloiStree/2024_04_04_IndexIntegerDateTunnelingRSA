using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;

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
                await SendReceiveMessages(clientWebSocket);
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
        Console.WriteLine("Sending message to WebSocket server...");
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
