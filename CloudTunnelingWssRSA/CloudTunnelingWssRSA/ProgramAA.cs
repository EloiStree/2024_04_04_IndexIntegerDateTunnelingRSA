using NetCoreServer;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

class ProgramAA
{


    static async Task Maindd(string[] args)
    {

        // create a task to run on parallel thread the following code
        var task = Task.Run(async () =>
       {
           Console.WriteLine("A");
           // Load the certificate from file
           X509Certificate2 certificate = new X509Certificate2(IID_WssCertificate.certificateFilePath, IID_WssCertificate.certificatePassword);
           IDD_WebSocketServer server = new IDD_WebSocketServer(certificate);
          // var context = new SslContext(SslProtocols.Tls13, new X509Certificate2(IID_WssCertificate.certificateFilePath, IID_WssCertificate.certificatePassword), (sender, certificate, chain, sslPolicyErrors) => true);



           await server.Start(IID_WssCertificate.uriServer);
           Console.WriteLine("AA");
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
