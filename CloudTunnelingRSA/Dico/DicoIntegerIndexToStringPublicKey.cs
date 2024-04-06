using CloudTunnelingRSA.Beans;

namespace CloudTunnelingRSA.Dico
{
    public class DicoIntegerIndexToStringPublicKey
    {

        public static DicoIntegerIndexToStringPublicKey Instance = new DicoIntegerIndexToStringPublicKey();
        public Dictionary<int, RsaPublicKeyRef> m_dicoIntegerIndexToRsa1024 = new Dictionary<int, RsaPublicKeyRef>();


       
        public void Add(int key, RsaPublicKeyRef value)
        {
            m_dicoIntegerIndexToRsa1024.Add(key, value);
        }
        public RsaPublicKeyRef Get(int key)
        {
            return m_dicoIntegerIndexToRsa1024[key];
        }
        public void Remove(int key)
        {
            m_dicoIntegerIndexToRsa1024.Remove(key);
        }
        public bool ContainsKey(int key)
        {
            return m_dicoIntegerIndexToRsa1024.ContainsKey(key);
        }
        public bool ContainsValue(RsaPublicKeyRef value)
        {
            return m_dicoIntegerIndexToRsa1024.ContainsValue(value);
        }
        public int Count()
        {
            return m_dicoIntegerIndexToRsa1024.Count;
        }
        public Dictionary<int, RsaPublicKeyRef>.KeyCollection GetKeys()
        {
            return m_dicoIntegerIndexToRsa1024.Keys;
        }
        public Dictionary<int, RsaPublicKeyRef>.ValueCollection GetValues()
        {
            return m_dicoIntegerIndexToRsa1024.Values;
        }
        public void Clear()
        {
            m_dicoIntegerIndexToRsa1024.Clear();
        }



    }
}
