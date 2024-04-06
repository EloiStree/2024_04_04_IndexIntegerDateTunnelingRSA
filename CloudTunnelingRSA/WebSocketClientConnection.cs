using System.Net.WebSockets;


public class WebSocketClientConnection
{

    public RsaConnectionHandShake m_handshake;
    public HttpListenerWebSocketContext m_httpConnection;
    public WebSocket m_webSocket;

    public bool IsRsaPublicKeyGiven()
    {
        return m_handshake.IsRsaPublicKeyGiven();
    }

}