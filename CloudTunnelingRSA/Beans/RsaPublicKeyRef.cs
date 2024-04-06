using System;

namespace CloudTunnelingRSA.Beans
{
    public class RsaPublicKeyRef
    {
        public static ulong m_indexCounter = 0;
        public ulong m_index = 0;
        public string m_publicKey = "";

        public RsaPublicKeyRef(in string publicKey)
        {
            m_index = m_indexCounter++;
            m_publicKey = publicKey;
        }
        public void SetPublickey(in string publicKey)
        {
            m_publicKey = publicKey;
        }
        public string GetPublicKey()
        {
            return m_publicKey;
        }
        public ulong GetObjectIntId()
        {
            return m_index;
        }
    }
}
