using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System;
using System.Security.Cryptography.Xml;
using static WebSocketServer;
using System.Security.Cryptography.X509Certificates;
using CloudTunnelingRSA.Dico;
using System.Collections.Generic;
using CloudTunnelingRSA.Beans;
using CloudTunnelingRSA;
using System.ComponentModel;


public class QuickTest
{

    public static string m_publicKey = "<RSAKeyValue><Modulus>vP7yDAkjkLrO7zqlaOlVpi3h7knD2xU4voEj3w9aJ9Pm/J0WADOOpnGcBc25VI7yuZuJZjsLuK9dz6aFVQR2+ZpT7H1aD/7qgXG10eIrOSu41ZIpcO26VDFcfsX1as7kmAQmLqFFTzcL2Yzv5Vz3982QeFy5Sx4MIRa26fbrKOE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    public static string m_tunnelToSigneMessage = Guid.NewGuid().ToString();
    public static bool m_isRsaValidated = false;
    public static bool m_saidHello;
    public static bool m_waitForSigneMessage;
}

partial class WebSocketServer
{

    public class AppConfig
    {

        public int m_portOfServer = 2501;
        public int m_portToListen = 2502;
        public bool m_useRebroadcastLastMessage = false;
        public bool m_displayIpAddresses = true;
        public bool m_useConsolePrint = false;
        public bool m_valueTypeIsByte = true;

        public static AppConfig Configuration = new AppConfig();
    }

    private const int BufferSize = 1024;
    private HttpListener httpListener;


    public async Task Start(string httpListenerPrefix, int udpListenerPort)
    {
        httpListener = new HttpListener();
        httpListener.Prefixes.Add(httpListenerPrefix);
        httpListener.Start();

        Console.WriteLine("WebSocket server is running...");


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

        Console.WriteLine($"Reqiest URI {webSocketContext.RequestUri}");
        //Console.WriteLine($"Origin {webSocketContext.Origin}");
        WebSocketClientConnection temp = new WebSocketClientConnection();
        temp.m_httpConnection = webSocketContext;
        temp.m_webSocket = webSocket;
        temp.m_handshake = null;

        RsaPublicKeyRef publicKeyRef = null;
        RsaConnectionHandShake connectionHandShake = null;

        try
        {
            byte[] buffer = new byte[BufferSize];
            byte[] receivedMessageBytes = new byte[32];


            Console.WriteLine($"A{" "}");

            while (webSocket.State == WebSocketState.Open)
            {
                Console.WriteLine($"B{" "}");
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                Console.WriteLine($"C{" "}");
                if (result.MessageType== WebSocketMessageType.Binary) {

                    ServerConsole.WriteLine($"> Received message bytes: {buffer.Length}");

                    if (connectionHandShake != null && connectionHandShake.WasReceivedValide())
                    {

                        Array.Copy(buffer, receivedMessageBytes, 32);
                        ServerConsole.WriteLine($"> Try to push bytes: {buffer.Length}");
                        await TryPushInRedirection(webSocket, receivedMessageBytes);
                        continue;
                    }else
                    {
                        await KillConnection(webSocketContext, "Validate RSA connection before sending bytes data.");
                        return;
                    }
                }
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        ServerConsole.WriteLine($"> Received message : {receivedMessage}");

                    if (connectionHandShake != null && connectionHandShake.WasReceivedValide())
                    {
                        ServerConsole.WriteLine($"> Try to push text: {receivedMessage}");
                        await TryPushInRedirection(webSocket, receivedMessage);
                        continue;
                    }
                    else {

                        if (receivedMessage.IndexOf("SIGNED:") == 0) { 
                            string signedMessage = receivedMessage.Substring("SIGNED:".Length).Trim(' ');
                            connectionHandShake.m_signMessageReceived = signedMessage;
                            bool isValide = connectionHandShake.ComputeCurrentValidityThenReturnIsValide();
                            if (!isValide)
                            {
                                await KillConnection(webSocketContext, "Signature not valid");
                                return;
                            }
                            else
                            {
                                await MessageBack(webSocket, "RSA:Verified");
                                await MessageBack(webSocket, "Congratulation. Welcome to the server. Make yourself as at your home. :)\n\n\n");

                                DicoWebSocketClientConnection.Instance.Add(publicKeyRef.GetObjectIntId(), temp);
                            }

                        }

                        if (publicKeyRef == null || connectionHandShake == null)
                        {

                            if (receivedMessage.Length > 5 && receivedMessage.IndexOf("Hello ") == 0)
                            {
                                string givenPublicKey = receivedMessage.Substring("Hello ".Length).Trim(' ');

                                if (!DicoRefRsaPublicKey.Instance.ContainsKey(givenPublicKey))
                                {
                                    await KillConnection(webSocketContext, "Public key not registered " + givenPublicKey);
                                    return;
                                }
                                publicKeyRef = DicoRefRsaPublicKey.Instance.Get(givenPublicKey);
                                Console.WriteLine($"Connection start with public key {publicKeyRef.GetObjectIntId()}:{publicKeyRef.GetPublicKey()}");
                                connectionHandShake = DicoRsaConnectionHandShake.Instance.CreateOrSetThenGet(publicKeyRef);

                                byte[] reply = Encoding.UTF8.GetBytes("SIGNIN:" + connectionHandShake.m_messageSentToClient);
                                await webSocket.SendAsync(new ArraySegment<byte>(reply), WebSocketMessageType.Text, true, CancellationToken.None);

                            }
                            else {
                                await KillConnection(webSocketContext, "Waiting mandatory 'Hello '"); return;
                            }
                        }
                        else
                        {

                            if (connectionHandShake == null) {
                                await KillConnection(webSocketContext, "Handshake is null");
                                return;
                            }

                            if (!connectionHandShake.IsSignedReceived())
                            {
                                
                            }
                        }
                    }


                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {

                    await KillConnection(webSocketContext, "Connection closed by client");
                }
            }
        }catch(Exception ex)
        {

            if(publicKeyRef!=null )
               DicoWebSocketClientConnection.Instance.Remove(publicKeyRef.GetObjectIntId());
            if (webSocketContext!=null && webSocketContext.WebSocket!=null)
                await KillConnection(webSocketContext, "Exception: " + ex.StackTrace);
            ServerConsole.WriteLine($"Error: {ex.Message}");
        }
        Console.WriteLine($"End Connection {webSocketContext.RequestUri}");
    }


    private WebSocketClientConnection m_serverTargetClient;

    public WebSocketClientConnection GetHandShake()
    {
        if (m_serverTargetClient == null)
        {
            ServerConsole.WriteLine("Server handshake is null");
            string target =ServerRsaKey.PublicKey;
            if (string.IsNullOrEmpty(target))
                return null;

            ServerConsole.WriteLine("Server handshake has target:"+target);

            if (DicoRefRsaPublicKey.Instance.ContainsKey(target))
            {
                RsaPublicKeyRef rsaPublicKeyRef = DicoRefRsaPublicKey.Instance.Get(target);
                ServerConsole.WriteLine("Server handshake has target is registered.");
               
                RsaConnectionHandShake serverHandshake = DicoRsaConnectionHandShake.Instance.GetIfExistsOrNull(rsaPublicKeyRef);

                if (serverHandshake == null)
                    return null;
                ServerConsole.WriteLine("Server handshake is instanciated");
                if (!serverHandshake.WasReceivedValide())
                    return null;
                ServerConsole.WriteLine("Server handshake is instanciated and valide");


                WebSocketClientConnection clientConnection =
                DicoWebSocketClientConnection.Instance.Get(rsaPublicKeyRef.GetObjectIntId());
                if (clientConnection == null || clientConnection.m_webSocket == null)
                    return null;
                ServerConsole.WriteLine("Server valide and connected client ?");
                if (clientConnection.m_webSocket.State != WebSocketState.Open)
                    return null;
                ServerConsole.WriteLine("Server open :) ");

                m_serverTargetClient = clientConnection; 

            }
        }
        return m_serverTargetClient;
    }

    private Task TryPushInRedirection(WebSocket webSocket, string receivedMessage)
    {
        WebSocketClientConnection c= GetHandShake();
        if (c == null) { 
            ServerConsole.WriteLine("Not sent message because client target not there");
            return Task.CompletedTask;

        }

        ServerConsole.WriteLine("Sent message:"+ receivedMessage);
        c.m_webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(receivedMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
        
        return Task.CompletedTask;
    }
    private Task TryPushInRedirection(WebSocket webSocket, byte [] receivedMessage)
    {

        WebSocketClientConnection c = GetHandShake();
        if (c == null)
        {
            ServerConsole.WriteLine("Not sent message because client target not there");
            return Task.CompletedTask;

        }

        ServerConsole.WriteLine("Sent message:"+ receivedMessage.Length);
        c.m_webSocket.SendAsync(new ArraySegment<byte>(receivedMessage), WebSocketMessageType.Binary, true, CancellationToken.None);
        return Task.CompletedTask;
    }

    private static async Task MessageBack(WebSocket webSocket, string messageCallBack)
 {
        if (string.IsNullOrEmpty(messageCallBack))
            return;
        if (webSocket==null || webSocket.State!=WebSocketState.Open)
        {
            return;
        }

        ServerConsole.WriteLine($"Message back: {messageCallBack}");
        byte[] reply = Encoding.UTF8.GetBytes(messageCallBack);
    await webSocket.SendAsync(new ArraySegment<byte>(reply), WebSocketMessageType.Text, true, CancellationToken.None);
}
private static async Task KillConnection(HttpListenerWebSocketContext webSocketContext, string message)
    {
        if(string.IsNullOrEmpty(message))
            return;
        if (webSocketContext == null )
        {
            return;
        }
        if (webSocketContext.WebSocket == null || webSocketContext.WebSocket.State != WebSocketState.Open)
        {
            return;
        }

        await MessageBack(webSocketContext.WebSocket, message);
        ServerConsole.WriteLine($"Kill connection: {message}");
        await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }


    public void Stop()
    {
        httpListener.Stop();
        httpListener.Close();
    }
}

public class TimeWatch
{
    public static DateTime m_startTime;
    public static DateTime m_endTime;

    public static void Start() { m_startTime = DateTime.Now; }
    public static void End() { m_endTime = DateTime.Now; }
    public static double GetSeconds() { return (m_endTime - m_startTime).TotalSeconds; }
}


class Program
{
    public static string m_configFileRelativePath = "ConfigBroadcaster.json";
    static async Task Main(string[] args)
    {
        if (!File.Exists("IdToPublicKey.txt"))
        {
            string dirRoot = Directory.GetCurrentDirectory();
            string[] strings = Directory.GetFiles(dirRoot, "*", SearchOption.AllDirectories);
            StringBuilder strinbBuilder = new StringBuilder();

            strings = strings.Where(s => s.EndsWith("RSA_PUBLIC_XML.txt")).ToArray();
            foreach (string s in strings)
            {
                string pathFolder = Path.GetDirectoryName(s);
                //Console.WriteLine(s);
                int lastIndex = pathFolder.LastIndexOf('\\');
                if (lastIndex <= -1)
                    continue;
                string index = pathFolder.Substring(lastIndex + 1);
                Console.WriteLine(index);

                if (int.TryParse(index, out int i))
                {
                    string publicKey = File.ReadAllText(s);
                    //Console.WriteLine(pathFolder);
                    Console.WriteLine(i+":"+publicKey);

                    strinbBuilder.Append("\n➤");
                    strinbBuilder.Append(i);
                    strinbBuilder.Append("☗");
                    strinbBuilder.Append(publicKey);
                    PushInRegsiterIndexRsa(i, publicKey);

                }


            }
            File.WriteAllText("IdToPublicKey.txt", strinbBuilder.ToString());
        }
        else { 
           string text= File.ReadAllText("IdToPublicKey.txt");
           text.Split("\n➤").ToList().ForEach(s =>
           {
               string [] dualSplit = s.Replace("\n➤","").Split('☗');
               if(dualSplit.Length==2)
               {
                   Console.WriteLine(dualSplit[0] + ":"+ dualSplit[1]);
                   if (int.TryParse(dualSplit[0], out int i)) {

                       PushInRegsiterIndexRsa(i, dualSplit[1]);
                   }
               }
           });
        }

        Console.WriteLine("Index register: "+string.Join(",",DicoIntegerIndexToStringPublicKey.Instance.GetKeys() ));


        if (!Directory.Exists("PublicKeyTarget"))
            Directory.CreateDirectory("PublicKeyTarget");

        if (!File.Exists("PublicKeyTarget/Redirection.txt"))
        {
            File.WriteAllText("PublicKeyTarget/Redirection.txt", QuickTest.m_publicKey);
        }

        string broadcastTotext = File.ReadAllText("PublicKeyTarget/Redirection.txt");

        ServerRsaKey.PublicKey = broadcastTotext;

        ServerConsole.WriteLine("\n\n\n\nRedirection to : " + ServerRsaKey.PublicKey);



        if (!File.Exists(m_configFileRelativePath))
            File.WriteAllText(m_configFileRelativePath, JsonConvert.SerializeObject(AppConfig.Configuration));

        string configUsed = File.ReadAllText(m_configFileRelativePath);
        Console.WriteLine(configUsed);
        AppConfig.Configuration = JsonConvert.DeserializeObject<AppConfig>(configUsed);


        if (AppConfig.Configuration.m_displayIpAddresses)
            NetworkInfo.DisplayConnectedLocalIPs();


        HideWindowTool.MinimizeConsoleWindow();
        string httpListenerPrefix = $"http://*:{AppConfig.Configuration.m_portOfServer}/";
        int udpListenerPort = AppConfig.Configuration.m_portToListen;

        WebSocketServer server = new WebSocketServer();
        await server.Start(httpListenerPrefix, udpListenerPort);

        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();

        server.Stop();
    }

    public  static void PushInRegsiterIndexRsa(int i, string publicKey)
    {
        RsaPublicKeyRef r = new RsaPublicKeyRef(in publicKey);
        DicoRefRsaPublicKey.Instance.Add(publicKey, r);
        DicoIntegerIndexToStringPublicKey.Instance.Add(i, r);

        ServerConsole.WriteLine($"RSA#{i}#{publicKey}");
    }
}
