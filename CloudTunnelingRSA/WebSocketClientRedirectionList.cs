using CloudTunnelingRSA.Beans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTunnelingRSA
{
    public class WebSocketClientRedirectionList
    {
        public static WebSocketClientRedirectionList Instance = new WebSocketClientRedirectionList();
        public Dictionary<ulong, RsaPublicKeyRef> m_redirectionRsaPublicKey = new Dictionary<ulong, RsaPublicKeyRef>();
        public Dictionary<int,int> m_redirectionIndex = new Dictionary<int, int>();


        public List<WebSocketClientConnection> m_clientToRedirectTo = new List<WebSocketClientConnection>();

        
        public void AddClientToRedirectTo(WebSocketClientConnection client)
        {
            m_clientToRedirectTo.Add(client);
        }
        public void RemoveClientToRedirectTo(WebSocketClientConnection client)
        {
            m_clientToRedirectTo.Remove(client);
        }
        public void RemoveClientOffline()
        {
            m_clientToRedirectTo.RemoveAll( client =>client!=null &&  client.IsConnected() == false);

        }


        public void AddIndex(int index)
        {
            m_redirectionIndex.Add(index, index);
        }
        public void RemoveIndex(int index)
        {
            m_redirectionIndex.Remove(index);
        }
        public bool ContainsIndex(int index)
        {
            return m_redirectionIndex.ContainsKey(index);
        }

        public void Add( RsaPublicKeyRef rsaKey)
        {
            m_redirectionRsaPublicKey.Add(rsaKey.GetObjectMemoryId(), rsaKey);
        }
        public void Remove(RsaPublicKeyRef rsaKey)
        {
            m_redirectionRsaPublicKey.Remove(rsaKey.GetObjectMemoryId());
        }

        public bool ContainsKey(ulong value)
        {
            return m_redirectionRsaPublicKey.ContainsKey(value);
        }


        public bool ContainsKey(RsaPublicKeyRef value)
        {
            return m_redirectionRsaPublicKey.ContainsKey(value.GetObjectMemoryId());
        }

        public List<WebSocketClientConnection> GetList()
        {
            return m_clientToRedirectTo;
        }

        public int ClientConnectedCount()
        {
            return m_clientToRedirectTo.Count;
        }
    }
}
