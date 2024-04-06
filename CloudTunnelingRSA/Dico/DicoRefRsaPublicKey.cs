using CloudTunnelingRSA.Beans;

namespace CloudTunnelingRSA.Dico
{
    public class DicoRefRsaPublicKey
    {

        public static DicoRefRsaPublicKey Instance = new DicoRefRsaPublicKey();
        public Dictionary<string, RsaPublicKeyRef> m_dicoRsaPublicKey = new Dictionary<string, RsaPublicKeyRef>();

        public void Add(string key, RsaPublicKeyRef value)
        {
            m_dicoRsaPublicKey.Add(key, value);
        }
        public RsaPublicKeyRef Get(string key)
        {
            return m_dicoRsaPublicKey[key];
        }
        public void Remove(string key)
        {
            m_dicoRsaPublicKey.Remove(key);
        }
        public bool ContainsKey(string key)
        {
            return m_dicoRsaPublicKey.ContainsKey(key);
        }
        public bool ContainsValue(RsaPublicKeyRef value)
        {
            return m_dicoRsaPublicKey.ContainsValue(value);
        }
        public int Count()
        {
            return m_dicoRsaPublicKey.Count;
        }
        public Dictionary<string, RsaPublicKeyRef>.KeyCollection GetKeys()
        {
            return m_dicoRsaPublicKey.Keys;
        }
        public Dictionary<string, RsaPublicKeyRef>.ValueCollection GetValues()
        {
            return m_dicoRsaPublicKey.Values;
        }
        public void Clear()
        {
            m_dicoRsaPublicKey.Clear();
        }

        public RsaPublicKeyRef GetOrCreate(string rsaKey)
        {
            if (ContainsKey(rsaKey))
            {
                return Get(rsaKey);
            }
            else
            {
                RsaPublicKeyRef rsaPublicKeyRef = new RsaPublicKeyRef(rsaKey);
                Add(rsaKey, rsaPublicKeyRef);
                return rsaPublicKeyRef;
            }
        }
    }
}
