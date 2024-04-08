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

    public bool IsConnected()
    {
        return m_webSocket!=null && m_webSocket.State == WebSocketState.Open;
    }
}