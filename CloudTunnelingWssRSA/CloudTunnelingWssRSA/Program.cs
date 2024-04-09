using System;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // WebSocket server endpoint
        Uri serverUri = new Uri("wss://localhost:8080");

        // Path to your certificate file (.cer, .pfx, .p12, etc.)
        string certificateFilePath = "path_to_your_certificate_file";

        // Password for the certificate file, if applicable (only for .pfx or .p12 files)
        string certificatePassword = "certificate_password";

        // Load the certificate from file
        X509Certificate2 certificate;

        if (!string.IsNullOrEmpty(certificatePassword))
        {
            certificate = new X509Certificate2(certificateFilePath, certificatePassword);
        }
        else
        {
            certificate = new X509Certificate2(certificateFilePath);
        }

        // Create client WebSocket instance
        using (var clientWebSocket = new ClientWebSocket())
        {
            // Set the WebSocket client options
            clientWebSocket.Options.ClientCertificates.Add(certificate);

            // Connect to the WebSocket server
            await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);

            // Send and receive messages (implement your logic here)
            await SendReceiveMessages(clientWebSocket);

            // Close the WebSocket connection
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected", CancellationToken.None);
        }

        // Dispose the certificate object when done
        certificate.Dispose();
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
