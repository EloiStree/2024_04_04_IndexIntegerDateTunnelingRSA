using CloudTunnelingRSA.Beans;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;



public class RsaConnectionHandShake
    {
        public RsaPublicKeyRef m_publicRsaKeyReceived;
        public string m_uniqueMessageSentToClient = Guid.NewGuid().ToString();
        public string m_signMessageReceived = "";
        public bool m_isValid = false;
        public DateTime m_lastActivity = DateTime.UtcNow;

   

    public RsaConnectionHandShake(RsaPublicKeyRef key)
        {
            m_publicRsaKeyReceived = key;
            m_uniqueMessageSentToClient = Guid.NewGuid().ToString();
            m_signMessageReceived = "";
            m_isValid = false;
        }
        public bool IsRsaPublicKeyGiven()
        {
            return m_publicRsaKeyReceived.m_publicKey.Length > 0;
        }
        public bool IsSignedReceived()
        {
            return m_uniqueMessageSentToClient.Length > 0;
        }
        public bool WasReceivedValide()
        {
            return m_isValid;
        }
        public void SetValide(bool isValide)
        {
            m_isValid = isValide;
        }

        public bool ComputeCurrentValidityThenReturnIsValide()
        {
            m_isValid= CryptoTools.VerifySignature(m_uniqueMessageSentToClient, m_signMessageReceived, m_publicRsaKeyReceived);
            return m_isValid;
        }

    public void UpdateLastActivity()
    {
        m_lastActivity = DateTime.UtcNow;
    }
    public void GetSecondsSinceLastActivity(out double seconds)
    {
        seconds = (DateTime.UtcNow - m_lastActivity).TotalSeconds;
    }

    public RsaPublicKeyRef GetPublicKey()
    {
        return m_publicRsaKeyReceived;
    }
}




public class DicoOwnerListener {

    public static DicoOwnerListener Instance = new DicoOwnerListener();

    public Dictionary<ulong, List<OwnerListener>> m_listeners = new Dictionary<ulong, List<OwnerListener>>();


    public void GetListeners(ulong rsaKeyId, out List<OwnerListener> listeners)
    {
        if (m_listeners.ContainsKey(rsaKeyId))
        {
            listeners = m_listeners[rsaKeyId];
        }
        else
        {
            listeners = null;
        }
    }

    public void AddListener(ulong key, OwnerListener listener)
    {
        if (m_listeners.ContainsKey(key))
        {

            m_listeners[key].Remove(listener);
            m_listeners[key].Remove(listener);
            m_listeners[key].Add(listener);
        }
        else
        {
            m_listeners.Add(key, new List<OwnerListener>());
            m_listeners[key].Add(listener);
        }
    }


    public int m_countAtLastRemoveOfActiveConnection;
    public void RemoveInactive() {

        m_countAtLastRemoveOfActiveConnection = 0;
        List<ulong> keys = m_listeners.Keys.ToList();
        foreach (ulong key in keys)
        {
            List<OwnerListener> listeners = m_listeners[key];
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                if (listeners[i]==null
                    || listeners[i].m_connectionForCallback == null
                    || listeners[i].m_connectionForCallback.WebSocket == null
                    || listeners[i].m_connectionForCallback.WebSocket.State !=WebSocketState.Open
                    )
                {
                    listeners.RemoveAt(i);
                }
            }
            m_countAtLastRemoveOfActiveConnection+= listeners.Count;
        }
    }

    public  int GetListenerCount()
    {
        return m_countAtLastRemoveOfActiveConnection;
    }
}

public class  OwnerListener
{
    public HttpListenerWebSocketContext m_connectionForCallback;

    public void PushBack(byte[] data)
    {

        if (m_connectionForCallback == null)
            return;
        m_connectionForCallback.WebSocket.SendAsync(
            new ArraySegment<byte>(data),
            WebSocketMessageType.Binary,
            true, System.Threading.CancellationToken.None);

    }
    public void PushBackUTF8(string dataUft8)
    {

        byte[] utf8 = Encoding.UTF8.GetBytes(dataUft8);
        if (m_connectionForCallback == null)
            return;
        m_connectionForCallback.WebSocket.SendAsync(
            new ArraySegment<byte>(utf8),
            WebSocketMessageType.Text,
            true, System.Threading.CancellationToken.None);

    }

}