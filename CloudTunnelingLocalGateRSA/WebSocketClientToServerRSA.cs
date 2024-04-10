﻿using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;

public class WebSocketClientToServerRSA
{
    public static string m_serverUri = "ws://81.240.94.97:2501";
    public static string m_privateKey = "<RSAKeyValue><Modulus>vP7yDAkjkLrO7zqlaOlVpi3h7knD2xU4voEj3w9aJ9Pm/J0WADOOpnGcBc25VI7yuZuJZjsLuK9dz6aFVQR2+ZpT7H1aD/7qgXG10eIrOSu41ZIpcO26VDFcfsX1as7kmAQmLqFFTzcL2Yzv5Vz3982QeFy5Sx4MIRa26fbrKOE=</Modulus><Exponent>AQAB</Exponent><P>x5+b84t6DU7dmRnZbg6nK5eLyGseIyDVodarQ8f7C4kCTfgYG7WW89X1cU//jMsj3mjQntOjJF2BkhtX/HWO0w==</P><Q>8l77YEBBJiLo6yuFDZLWRyjYJsEvuE3/MQvSwXtY2Hb7BM+ynhIcncs6jGmUuSSNoXhQ877CeD2sOJbGV+Ng+w==</Q><DP>J98nZRO8wx+3fzb8iNEAbuKMFvHeSSHrybF478bny7wH687b8dzpU7aumX1jC5ofhfLliHO5KDBNCwPPJSvN5Q==</DP><DQ>OzKVxUmMYAswxpfHlKwjqBfCy5xt0l9CkDEqFdXRunU9FEzCfLdBxAyqTTdQevQBn8mqRA54ozO1B9FTuo2v1w==</DQ><InverseQ>K+5TNsF1zM4SeFX8Pd7OcsB3yYP0VkCCawyeQxjm3GQbQd805JnqCoaAnAiuM5N49jonQXuJMjYqgxT0JWh2VA==</InverseQ><D>oJ3J9pCNuSIJWyXsDQy/zUqRB4GJAVc3si7t3VOeutpLI8QcPm+Se8FxZz0+k64oebTFQCxN+daPUzmhdm8k6+OqoYV/gHCrWbEQMAKkavT3rxtlJbkWkFgqNxmMQA2/2feC0ESbavtZemBLOP7p+VVr/cYu6DzpUNr5+FVhD0E=</D></RSAKeyValue>";
    public static string m_publicKey = "<RSAKeyValue><Modulus>vP7yDAkjkLrO7zqlaOlVpi3h7knD2xU4voEj3w9aJ9Pm/J0WADOOpnGcBc25VI7yuZuJZjsLuK9dz6aFVQR2+ZpT7H1aD/7qgXG10eIrOSu41ZIpcO26VDFcfsX1as7kmAQmLqFFTzcL2Yzv5Vz3982QeFy5Sx4MIRa26fbrKOE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    public static byte[] SignData(byte[] data, RSAParameters privateKey)
    {
        using (RSA rsa = RSA.Create())
        {
            rsa.ImportParameters(privateKey);
            return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
    public static bool VerifySignature(byte[] data, byte[] signature, RSAParameters publicKey)
    {
        using (RSA rsa = RSA.Create())
        {
            rsa.ImportParameters(publicKey);
            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }


    public static Queue<string> m_toSendToTheServerUTF8 = new Queue<string>();
    public static Queue<byte[]> m_toSendToTheServerBytes = new Queue<byte[]>();
    public static Queue<byte[]> m_toSendToTheServerUTF8AsByteRecevied = new Queue<byte[]>();
    

    public static void SentToServerAsBinary(byte[] data)
    {
        m_toSendToTheServerBytes.Enqueue(data);
    }
    public static void SentToServerAsUTF8(string data)
    {
        m_toSendToTheServerUTF8.Enqueue(data);
    }
    public static void SentToServerAsUTF8AsByte(byte[] data)
    {
        m_toSendToTheServerUTF8AsByteRecevied.Enqueue(data);
    }
    public async Task ConnectAndRun()
    {



        while (true)
        {
            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                try
                {
                    m_messageToSignedReceived = "";
                    m_connectionEstablishedAndVerified = false;
                    Console.WriteLine($"Connecting to server: {m_serverUri}");
                    await webSocket.ConnectAsync(new Uri(m_serverUri), CancellationToken.None);

                    Task.Run(() => ReceiveMessages(webSocket));





                    while (webSocket.State == WebSocketState.Open)
                    {

                        if (!m_connectionEstablishedAndVerified)
                        {
                            byte[] b = Encoding.UTF8.GetBytes("Hello "+ m_publicKey);
                            await webSocket.SendAsync(new ArraySegment<byte>(b), WebSocketMessageType.Text, true, CancellationToken.None);

                            while (m_messageToSignedReceived.Length == 0)
                            {
                                await Task.Delay(20);
                            }



                            RSA rsa = RSA.Create();

                            rsa.KeySize = 1024;
                            rsa.FromXmlString(m_privateKey);
                            RSAParameters privateKey = rsa.ExportParameters(true);
                            RSAParameters publicKey = rsa.ExportParameters(false);

                            string messageToSigne = m_messageToSignedReceived;
                            byte[] data = Encoding.UTF8.GetBytes(messageToSigne);
                            byte[] signature = SignData(data, privateKey);
                            var signatureBase64 = Convert.ToBase64String(signature);
                            string sent = "RSA:" + signatureBase64;
                            byte[] signatureBytes = Encoding.UTF8.GetBytes(sent);
                            Console.WriteLine($"Sent message to server: {sent}");
                            Console.WriteLine($"Sent message to server: {messageToSigne}");
                            await webSocket.SendAsync(new ArraySegment<byte>(signatureBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            // create a signe message
                            byte[] bb = Encoding.UTF8.GetBytes(messageToSigne);
                            await webSocket.SendAsync(new ArraySegment<byte>(bb), WebSocketMessageType.Text, true, CancellationToken.None);

                            while (!m_connectionEstablishedAndVerified)
                            {
                                await Task.Delay(20);
                            }

                        }



                        while (m_toSendToTheServerUTF8.Count > 0)
                        {
                            byte[] messageBytes = Encoding.UTF8.GetBytes(m_toSendToTheServerUTF8.Dequeue());
                            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        while (m_toSendToTheServerUTF8AsByteRecevied.Count > 0)
                        {
                            byte[] messageBytes = m_toSendToTheServerUTF8AsByteRecevied.Dequeue();
                            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        while (m_toSendToTheServerBytes.Count > 0)
                        {
                            byte[] messageBytes = m_toSendToTheServerBytes.Dequeue();
                            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Binary, true, CancellationToken.None);
                        }
                        await Task.Delay(1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WebSocket error: {ex.Message}");
                    Console.WriteLine("Reconnecting in 5 seconds...");
                    await Task.Delay(5000);
                }
            }
        }
    }

    public static string m_messageToSignedReceived = "";
    public static bool m_connectionEstablishedAndVerified = false;
    private async Task ReceiveMessages(ClientWebSocket webSocket)
    {
        byte[] buffer = new byte[4096];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message from server: {receivedMessage}");
                    if (!m_connectionEstablishedAndVerified)
                    {
                        if (receivedMessage.Contains("SIGNEHERE:"))
                        {
                            m_messageToSignedReceived = receivedMessage.Replace("SIGNEHERE:", "");
                        }
                        if (receivedMessage.Contains("RSA:Verified"))
                        {
                            m_connectionEstablishedAndVerified = true;
                        }
                    }

    
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");

            // Handle reconnection logic
            Console.WriteLine("Reconnecting in 5 seconds...");
            await Task.Delay(5000);
        }
    }

}
