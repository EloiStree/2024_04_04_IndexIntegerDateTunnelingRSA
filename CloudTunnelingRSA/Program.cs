﻿using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography.Xml;
using static WebSocketServer;
using System.Security.Cryptography.X509Certificates;
using CloudTunnelingRSA.Dico;
using System.Collections.Generic;
using CloudTunnelingRSA.Beans;
using CloudTunnelingRSA;
using System.ComponentModel;
using CloudTunnelingRSA.IndexIntegerDate;



public class AllowGuestRSA {
    public static bool Allow = true;


}
public class StaticConsoleUtility
{
    public static bool DebugIID = true;
    public  static bool DebugClientConnectionError = true;
    public static bool DebugRsaHandshake = true;
    internal static bool DebugReceivedTextMessage=true;
}

//public class DicoIpAddressLimit {

//    public bool m_maxUserByIP =3;
//    public Dictionary<string , WebSocketClientConnection> m_connectionByIp= 
//        new Dictionary<string, WebSocketClientConnection>();

//    public void Add(string ip, WebSocketClientConnection connection) { 
//        m_connectionByIp.Add(ip, connection);
//    }

//}

//public class QuickTest
//{

//    public static string m_publicKey = "<RSAKeyValue><Modulus>vP7yDAkjkLrO7zqlaOlVpi3h7knD2xU4voEj3w9aJ9Pm/J0WADOOpnGcBc25VI7yuZuJZjsLuK9dz6aFVQR2+ZpT7H1aD/7qgXG10eIrOSu41ZIpcO26VDFcfsX1as7kmAQmLqFFTzcL2Yzv5Vz3982QeFy5Sx4MIRa26fbrKOE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
//    public static string m_tunnelToSigneMessage = Guid.NewGuid().ToString();
//    public static bool m_isRsaValidated = false;
//    public static bool m_saidHello;
//    public static bool m_waitForSigneMessage;
//}

partial class WebSocketServer
{

    public class AppConfig
    {

        public int m_portOfServer = 2501;
        public int m_portToListen = 2502;
        public int m_portToBroadcastLocalOutUdp= 2503;
        public bool m_useRebroadcastLastMessage = false;
        public bool m_displayIpAddresses = true;
        public bool m_useConsolePrint = false;
        public bool m_valueTypeIsByte = true;
        public bool m_allowToListenAtChanged=true;

        public static AppConfig Configuration = new AppConfig();
        internal bool m_useConsoleStatePrint=true;
    }

    private const int BufferSize = 4080;
    private HttpListener httpListener;


    public async Task Start(string httpListenerPrefix)
    {

        
        httpListener = new HttpListener();
        httpListener.Prefixes.Add(httpListenerPrefix);
        try
        {

            httpListener.Start();
        }
        catch (Exception ex) { 
            Console.WriteLine("SERVER FAIL TO LAUNCH: Need to be admin and single on the given port");
            Environment.Exit(0);
            return;
        }

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

//        ServerConsole.WriteLine($"Reqiest URI {webSocketContext.RequestUri}");
        //ServerConsole.WriteLine($"Origin {webSocketContext.Origin}");
        WebSocketClientConnection temp = new WebSocketClientConnection();
        temp.m_httpConnection = webSocketContext;
        temp.m_webSocket = webSocket;
        temp.m_handshake = null;

        RsaPublicKeyRef publicKeyRef = null;
        RsaConnectionHandShake connectionHandShake = null;
        int indexLockedOn = 0; 
        bool isIndexLocked = false;
        CancellationToken token = default(CancellationToken);

        try
        {
            byte[] buffer = new byte[BufferSize];
            byte[] receivedMessageBytes = new byte[16];

            bool firstOpen = true;

            while (webSocket.State == WebSocketState.Open)
            {
                if (firstOpen) {
                    firstOpen = false;
                    Console.WriteLine($"New Connection: {webSocketContext.RequestUri} {webSocketContext.Origin} ");

                }
                if (QuitIfWebsocketClosed(webSocket)) break;

                if (connectionHandShake!=null)
                connectionHandShake.UpdateLastActivity();

                WebSocketReceiveResult result = null;
                try {
                    if (buffer == null || webSocket ==null|| buffer.Length <= 12)
                        continue;

                    if (!IsWebsocketOpen(webSocket)) break;
                    result= await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    if (result == null || !result.EndOfMessage || result.Count==0 ) {
                        await Task.Delay(1);
                        continue;
                    }
                
                }
                catch(Exception ex)
                {

                   // ServerConsole.WriteLine($"Exeception: Stack:{ex.StackTrace} M:{ex.Message} S:{ ex.Source}");
                    await Task.Delay(1);
                    continue;
                }
                if (result == null) { 
                    await Task.Delay(1);
                    continue;
                }
                //ServerConsole.WriteLine($"Received message type: {result.MessageType} {result.Count}");
                ByteReceivedCount.Instance.AddByteCount(result.Count);


                //ServerConsole.WriteLine($"BIT AS RECEIVED{string.Join(" ", receivedMessageBytes)}");
                if (result.MessageType== WebSocketMessageType.Binary) {

                    if (QuitIfWebsocketClosed(webSocket)) break;
                    await BufferToIndexIntegerDate(webSocket, connectionHandShake, webSocketContext, buffer, receivedMessageBytes, indexLockedOn);
                    continue;
                }
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    if (QuitIfWebsocketClosed(webSocket)) break;
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if(StaticConsoleUtility.DebugReceivedTextMessage)
                    ServerConsole.WriteLine($">T Received message : {receivedMessage}");


                    if (connectionHandShake != null && connectionHandShake.WasReceivedValide())
                    {
                        if (receivedMessage.StartsWith("b|"))
                        {
                            receivedMessage = receivedMessage.Substring(2);
                            byte[] bytes = Convert.FromBase64String(receivedMessage);

                            await BufferToIndexIntegerDate(webSocket, connectionHandShake, webSocketContext, bytes, receivedMessageBytes, indexLockedOn);
                            continue;
                        }
                        if (receivedMessage.StartsWith("i|"))
                        {
                            receivedMessage = receivedMessage.Substring(2);
                            if(int.TryParse(receivedMessage, out int value))
                            {
                                byte[] bytes = new byte[12];
                                BitConverter.GetBytes(value).CopyTo(bytes, 0);
                                BitConverter.GetBytes(GetTimeUTCAsLong()).CopyTo(bytes, 4);

                                await BufferToIndexIntegerDate(webSocket, connectionHandShake, webSocketContext, bytes, receivedMessageBytes, indexLockedOn);
                            }
                            continue;
                        }
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
                                DicoIntegerIndexToStringPublicKey.Instance.Get(publicKeyRef, out bool found, out int index);



                                if (!found) { 
                                    await KillConnection(webSocketContext, "Integer Index not found for the RSA Key given");
                                    return;
                                }

                                await MessageBack(webSocket, "IndexLock:"+index);
                                indexLockedOn = index;
                                isIndexLocked = true;
                                await MessageBack(webSocket, "Congratulation. Welcome to the server. :)\n\n\n");
                                await MessageBack(webSocket, "Source code of server: https://github.com/EloiStree/2024_04_04_IndexIntegerDateTunnelingRSA:)\n");
                                await MessageBack(webSocket, "Default Python Gate: https://github.com/EloiStree/2024_05_11_GateIID_WS_Python:)\n");
                                await MessageBack(webSocket, "Default Unity Gate: https://github.com/EloiStree/2024_04_04_UnityServerTunnelingRSAUnity.git:)\n");

                                DicoWebSocketClientConnection.Instance.Add(publicKeyRef.GetObjectMemoryId(), temp);
                                DicoOwnerListener.Instance.AddListener(publicKeyRef.GetObjectMemoryId(), new OwnerListener() { m_connectionForCallback = webSocketContext });

                                if (AppConfig.Configuration.m_allowToListenAtChanged)
                                    await MessageBack(webSocket, "(Listen to valuelue change. This feature is temporary. The time to create a websocket dedicated to that.)");
                                


                                if (WebSocketClientRedirectionList.Instance.ContainsIndex(indexLockedOn))
                                    WebSocketClientRedirectionList.Instance.AddClientToRedirectTo(temp);
                                else if (WebSocketClientRedirectionList.Instance.ContainsKey(publicKeyRef))
                                    WebSocketClientRedirectionList.Instance.AddClientToRedirectTo(temp);
                            }

                        }

                        if (publicKeyRef == null || connectionHandShake == null)
                        {



                            if (receivedMessage.Length > 5 && receivedMessage.IndexOf("Hello ") == 0)
                            {


                                string givenPublicKey = receivedMessage.Substring("Hello ".Length).Trim(' ');


                                ConvertGivenKeyToRSAXML.TryParse(givenPublicKey, out bool foundAndConvert, out givenPublicKey);
                                if (StaticConsoleUtility.DebugRsaHandshake)
                                    Console.WriteLine($"Given public key {foundAndConvert}: {givenPublicKey}");


                                if (!DicoRefRsaPublicKey.Instance.ContainsKey(givenPublicKey))
                                {
                                    if (!AllowGuestRSA.Allow)
                                    {
                                        await KillConnection(webSocketContext, "Public key not registered " + givenPublicKey);
                                        return;
                                    }
                                    
                                    else { 

                                        ///REMINDER USE GUEST MODE IS A STRESS TEST MODE THAT SHOULD NOT BE USE BECAUSE IT IS HARD TO AVOID TROLLING IN THIS MODE.
                                        int index = DicoIntegerIndexToStringPublicKey.Instance.ClaimUnusedNegative();

                                        RsaPublicKeyRef r=  DicoRefRsaPublicKey.Instance.GetOrCreate(givenPublicKey);
                                        DicoIntegerIndexToStringPublicKey.Instance.Add(index, r);
                                        DicoGuestTracker.Instance.Add(index, r);
                                        //ADD A IP EQUALS ONE RSA AS GUEST. To AVOID SPAM AND DOS ATTACK when in GUEST MODE.


                                    }
                                }
                                publicKeyRef = DicoRefRsaPublicKey.Instance.Get(givenPublicKey);
                                if(StaticConsoleUtility.DebugRsaHandshake)
                                ServerConsole.WriteLine($"Connection start with public key {publicKeyRef.GetObjectMemoryId()}:{publicKeyRef.GetPublicKey()}");
                                connectionHandShake = new RsaConnectionHandShake(publicKeyRef);
                                DicoRsaConnectionHandShake.Instance.AddHandshake(publicKeyRef, connectionHandShake);

                                byte[] reply = Encoding.UTF8.GetBytes("SIGNIN:" + connectionHandShake.m_uniqueMessageSentToClient);
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
               DicoWebSocketClientConnection.Instance.Remove(publicKeyRef.GetObjectMemoryId());

            if (IsWebsocketOpen(webSocket)) { 
                await MessageBack(webSocket, "Exception: " + ex.StackTrace);
            }
            if (IsWebsocketOpen(webSocketContext))
                await KillConnection(webSocketContext, "Exception: " + ex.StackTrace);
            if (StaticConsoleUtility.DebugClientConnectionError) { 
                ServerConsole.WriteLine($"Error: {ex.StackTrace}");
            }
        }
        token.ThrowIfCancellationRequested();
        ServerConsole.WriteLine($"End Connection {webSocketContext.RequestUri}");
        Console.WriteLine("End Connection");
    }

    private bool IsWebsocketOpen(HttpListenerWebSocketContext webSocketContext)
    {
        return webSocketContext != null && IsWebsocketOpen(webSocketContext.WebSocket);
    }

    public bool IsWebsocketOpen(WebSocket webSocket)
    {
        return webSocket != null && (webSocket.State == WebSocketState.Connecting || webSocket.State == WebSocketState.Open);
    }

    private bool QuitIfWebsocketClosed(WebSocket webSocket)
    {
        return webSocket==null || webSocket.State != WebSocketState.Open;
    }

    private ulong GetTimeUTCAsLong()
    {
        return (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
       // return  (ulong)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
    }

    private async Task BufferToIndexIntegerDate(
       System.Net.WebSockets.WebSocket websocket,
       RsaConnectionHandShake connectionHandShake,
       HttpListenerWebSocketContext webSocketContext,
       byte[] buffer,
       byte[] receivedMessageBytes,
       int indexLockedOn
      )
    {


        Array.Copy(buffer, 0, receivedMessageBytes, 4, 12);
        await BufferToIndexIntegerDate(websocket, connectionHandShake, webSocketContext, receivedMessageBytes, indexLockedOn);
    }

        private async Task BufferToIndexIntegerDate(
         System.Net.WebSockets.WebSocket websocket,
         RsaConnectionHandShake connectionHandShake,
         HttpListenerWebSocketContext webSocketContext,
         byte[]  receivedMessageBytes,
         int indexLockedOn
        )
    {
        //ServerConsole.WriteLine($">BBBBBBBBBBB Received message bytes: {buffer.Length}");
        //ServerConsole.WriteLine($">Bytes: {string.Join(" ", buffer)}");

        if (connectionHandShake != null && connectionHandShake.WasReceivedValide())
        {
            BitConverter.GetBytes(indexLockedOn).CopyTo(receivedMessageBytes, 0);
            int value = BitConverter.ToInt32(receivedMessageBytes, 4);
            ulong timeStampUtc = BitConverter.ToUInt64(receivedMessageBytes, 8);
            if (StaticConsoleUtility.DebugIID)
               ServerConsole.WriteLine($"Index:{indexLockedOn} Value:{value} TimeStampUtc:{timeStampUtc}");
            DicoIndexIntegerDate.Instance.Set(
                    indexLockedOn, value, timeStampUtc, out bool changed);


            PushToLocalUdpExit(indexLockedOn, value, timeStampUtc);

            bool useNeedChangeAntiSpam=false;
            if (useNeedChangeAntiSpam) { 
                if (changed)
                {
                    // ServerConsole.WriteLine($"> Try to push bytes: {receivedMessageBytes}");
                    await TryPushInRedirection(websocket, receivedMessageBytes);
                    PushBackToListenerRSA.Instance.PushBackBytes(connectionHandShake.m_publicRsaKeyReceived, receivedMessageBytes);
                }
                else
                {
                    await KillConnection(webSocketContext, "Not change was detected. To avoid spam, you were disconnected");
                    return;
                }

            }
            else
            {
                await TryPushInRedirection(websocket, receivedMessageBytes);
                PushBackToListenerRSA.Instance.PushBackBytes(connectionHandShake.m_publicRsaKeyReceived, receivedMessageBytes);

            }


        }
        else
        {
            await KillConnection(webSocketContext, "Validate RSA connection before sending bytes data.");
            return;
        }
    }

    UdpClient udpClientOut = null;
    IPEndPoint endPointOut = null;
    private void PushToLocalUdpExit(int indexLockedOn, int value, ulong timeStampUtc)
    {
        if (AppConfig.Configuration.m_portToBroadcastLocalOutUdp <= 0)
            return;

        if (udpClientOut == null) { 
            udpClientOut = new UdpClient();
            int port = AppConfig.Configuration.m_portToBroadcastLocalOutUdp;
            endPointOut = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
           
        }

        byte[] bytes = new byte[16];
        BitConverter.GetBytes(indexLockedOn).CopyTo(bytes, 0);
        BitConverter.GetBytes(value).CopyTo(bytes, 4);
        BitConverter.GetBytes(timeStampUtc).CopyTo(bytes, 8);
        udpClientOut.Send(bytes, bytes.Length, endPointOut);

    }



    // private WebSocketClientConnection m_serverTargetClient;

    //public List<WebSocketClientConnection> GetHandShake()
    //{

    //    if(m_serverTargetClient!=null && m_serverTargetClient.m_webSocket.State!=WebSocketState.Open)
    //        m_serverTargetClient = null;

    //    if (m_serverTargetClient == null)
    //    {
    //        ServerConsole.WriteLine("Server handshake is null");
    //        string target =ServerRsaKey.PublicKey;
    //        if (string.IsNullOrEmpty(target))
    //            return null;

    //        ServerConsole.WriteLine("Server handshake has target:"+target);

    //        if (DicoRefRsaPublicKey.Instance.ContainsKey(target))
    //        {
    //            RsaPublicKeyRef rsaPublicKeyRef = DicoRefRsaPublicKey.Instance.Get(target);
    //            ServerConsole.WriteLine("Server handshake has target is registered.");

    //            List<RsaConnectionHandShake> serverHandshakeList = DicoRsaConnectionHandShake.Instance.GetIfExistsOrNull(rsaPublicKeyRef);
    //            if(serverHandshakeList == null || serverHandshakeList.Count==0)
    //                return null;

    //            RsaConnectionHandShake serverHandshake = serverHandshakeList[0];
    //            ServerConsole.WriteLine("Server handshake is instanciated");
    //            if (!serverHandshake.WasReceivedValide())
    //                return null;
    //            ServerConsole.WriteLine("Server handshake is instanciated and valide");


    //            WebSocketClientConnection clientConnection =
    //            DicoWebSocketClientConnection.Instance.Get(rsaPublicKeyRef.GetObjectMemoryId());


    //            if (clientConnection == null || clientConnection.m_webSocket == null)
    //                return null;
    //            ServerConsole.WriteLine("Server valide and connected client ?");
    //            if (clientConnection.m_webSocket.State != WebSocketState.Open)
    //                return null;
    //            ServerConsole.WriteLine("Server open :) ");

    //            m_serverTargetClient = clientConnection; 

    //        }
    //    }
    //    return m_serverTargetClient;
    //}

    private Task TryPushInRedirection(System.Net.WebSockets. WebSocket webSocket, string receivedMessage)
    {
        List<WebSocketClientConnection> cs= WebSocketClientRedirectionList.Instance.GetList();

        foreach (var c in cs)
        {
            if (c != null && c.m_webSocket.State == WebSocketState.Open)
            {

                //ServerConsole.WriteLine("Sent message:" + receivedMessage);
                c.m_webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(receivedMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        return Task.CompletedTask;
    }
    public bool m_usePushBackToListener = true;
    private Task TryPushInRedirection(WebSocket webSocket, byte[] receivedMessage)
    {
        byte[] copy = new byte[receivedMessage.Length];
        receivedMessage.CopyTo(copy, 0); 
        List<WebSocketClientConnection> cs = WebSocketClientRedirectionList.Instance.GetList();
        foreach (var c in cs)
        {
            if (c !=null && c.m_webSocket.State == WebSocketState.Open)
            {
                if (m_usePushBackToListener) { 
                    c.m_webSocket.SendAsync(new ArraySegment<byte>(copy), WebSocketMessageType.Binary, true, CancellationToken.None);
                    c.m_webSocket.SendAsync(new ArraySegment<byte>(new byte[0]), WebSocketMessageType.Binary, true, CancellationToken.None);
                    ServerConsole.WriteLine("Sent message:" + copy.Length);
                }
                

            }
        }
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
        //ServerConsole.WriteLine($"Message back: {messageCallBack}");
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

        try { 
            await MessageBack(webSocketContext.WebSocket, message);
            ServerConsole.WriteLine($"Kill connection: {message}");
            await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }catch(Exception e)
        {
            ServerConsole.WriteLine($"Kill connection Exception: {e.Message}");

        }
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

        //Launch task to do something every 5 seconds
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(5000);
                DebugServerState();
                FlushLostConnection();
            }
        });


        if (!File.Exists("IdToPublicKey.txt"))
        {
            string dirRoot = Directory.GetCurrentDirectory();
            string[] strings = Directory.GetFiles(dirRoot, "*", SearchOption.AllDirectories);
            StringBuilder strinbBuilder = new StringBuilder();

            strings = strings.Where(s => s.EndsWith("RSA_PUBLIC_XML.txt")).ToArray();
            foreach (string s in strings)
            {
                string pathFolder = Path.GetDirectoryName(s);
                //ServerConsole.WriteLine(s);
                int lastIndex = pathFolder.LastIndexOf('\\');
                if (lastIndex <= -1)
                    continue;
                string index = pathFolder.Substring(lastIndex + 1);
                ServerConsole.WriteLine(index);

                if (int.TryParse(index, out int i))
                {
                    string publicKey = File.ReadAllText(s);
                    //ServerConsole.WriteLine(pathFolder);
                    ServerConsole.WriteLine(i+":"+publicKey);

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
                   ServerConsole.WriteLine(dualSplit[0] + ":"+ dualSplit[1]);
                   if (int.TryParse(dualSplit[0], out int i)) {

                       PushInRegsiterIndexRsa(i, dualSplit[1]);
                   }
               }
           });
        }

        ServerConsole.WriteLine("Index register: "+string.Join(",",DicoIntegerIndexToStringPublicKey.Instance.GetKeys() ));


        if (!Directory.Exists("PublicKeyTarget"))
            Directory.CreateDirectory("PublicKeyTarget");

        if (!File.Exists("PublicKeyTarget/RedirectionIndex.txt"))
        {
            File.WriteAllText("PublicKeyTarget/RedirectionIndex.txt", "0");
        }

        
        string [] broadcastTotext = File.ReadAllText("PublicKeyTarget/RedirectionIndex.txt").Split(',');
        foreach (var item in broadcastTotext)
        {
            if(int.TryParse(item, out int index))
                WebSocketClientRedirectionList.Instance.AddIndex(index);
        }

        if (!File.Exists(m_configFileRelativePath))
            File.WriteAllText(m_configFileRelativePath, JsonConvert.SerializeObject(AppConfig.Configuration));

        string configUsed = File.ReadAllText(m_configFileRelativePath);
        ServerConsole.WriteLine(configUsed);
        AppConfig.Configuration = JsonConvert.DeserializeObject<AppConfig>(configUsed);


        if (AppConfig.Configuration.m_displayIpAddresses)
            NetworkInfo.DisplayConnectedLocalIPs();


        HideWindowTool.MinimizeConsoleWindow();
        string httpListenerPrefix = $"http://*:{AppConfig.Configuration.m_portOfServer}/";
        int udpListenerPort = AppConfig.Configuration.m_portToListen;

        WebSocketServer server = new WebSocketServer();
        await server.Start(httpListenerPrefix);



        ServerConsole.WriteLine("Press any key to stop the server...");
        Console.ReadKey();

        server.Stop();
    }

    private static void DebugServerState()
    {

        ServerStateConsole.WriteLine("> SERVER STATE");
        ServerStateConsole.WriteLine("> ------------------------ <");
        ServerStateConsole.WriteLine("> Key registered " + DicoIntegerIndexToStringPublicKey.Instance.GetValuesCount());
        if (AllowGuestRSA.Allow)
            ServerStateConsole.WriteLine("> Guest " + DicoGuestTracker.Instance.GetValuesCount());

        ServerStateConsole.WriteLine("> OwnerListener " + DicoOwnerListener.Instance.GetListenerCount());
        ServerStateConsole.WriteLine("> Handshake " + DicoRsaConnectionHandShake.Instance.GetValuesCount());
        ServerStateConsole.WriteLine("> Connected " + DicoWebSocketClientConnection.Instance.GetValuesCount());
        ServerStateConsole.WriteLine("> Connected Redirection " + WebSocketClientRedirectionList.Instance.ClientConnectedCount());
        ServerStateConsole.WriteLine($"> Received  " +
            $" {ByteReceivedCount.Instance.GetByteAsMegaByte()}MB" +
            $" {ByteReceivedCount.Instance.GetByteAsGigaByte()}GB" +
            $" {ByteReceivedCount.Instance.GetByteCount()}");

    }

    private static void FlushLostConnection()
    {
        WebSocketClientRedirectionList.Instance.RemoveClientOffline();
        List<ulong> toRemove = new List<ulong>();
        foreach (var item in DicoWebSocketClientConnection.Instance.GetKeys())
        {
            WebSocketClientConnection c = DicoWebSocketClientConnection.Instance.Get(item);
            if (c == null || c.m_webSocket == null || c.m_webSocket.State != WebSocketState.Open)
            {
                toRemove.Add(item);
            }
        
        }

        foreach (ulong i in toRemove)
        {
           WebSocketClientConnection connection= DicoWebSocketClientConnection.Instance.Get(i);
           DicoRsaConnectionHandShake.Instance.Remove(connection.m_handshake);
           DicoWebSocketClientConnection.Instance.Remove(i);


        }
            DicoOwnerListener.Instance.RemoveInactive();

        
        
    }

    public  static void PushInRegsiterIndexRsa(int i, string publicKey)
    {
        RsaPublicKeyRef r = new RsaPublicKeyRef(in publicKey);
        DicoRefRsaPublicKey.Instance.Add(publicKey, r);
        DicoIntegerIndexToStringPublicKey.Instance.Add(i, r);

        ServerConsole.WriteLine($"RSA#{i}#{publicKey}");
    }
}
