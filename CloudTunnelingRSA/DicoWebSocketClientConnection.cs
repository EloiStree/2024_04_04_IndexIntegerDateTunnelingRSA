using System.Collections.Concurrent;


public class DicoWebSocketClientConnection
{
    public static DicoWebSocketClientConnection Instance = new DicoWebSocketClientConnection();
    public ConcurrentDictionary<ulong, WebSocketClientConnection> m_connectedClients = new ConcurrentDictionary<ulong, WebSocketClientConnection>();

    public void Add(ulong key, WebSocketClientConnection value)
    {
        m_connectedClients.AddOrUpdate(key, value, (k, v) => value);
    }
    public WebSocketClientConnection Get(ulong key)
    {
        return m_connectedClients[key];
    }
    public void Remove(ulong key)
    {
        m_connectedClients.TryRemove(key, out _);
    }
    public bool ContainsKey(ulong key)
    {
        return m_connectedClients.ContainsKey(key);
    }

    public int Count()
    {
        return m_connectedClients.Count;
    }

    public void Clear()
    {
        m_connectedClients.Clear();
    }
}