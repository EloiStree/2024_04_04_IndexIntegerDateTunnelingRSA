namespace CloudTunnelingRSA.Dico
{
    public class DicoRsaConnectionStateValide
    {
        public Dictionary<string, RsaConnectionState> m_dicoRsaConnectionState = new Dictionary<string, RsaConnectionState>();

        public void Add(string key, RsaConnectionState rsaConnectionState)
        {
            m_dicoRsaConnectionState.Add(key, rsaConnectionState);
        }

        public RsaConnectionState Get(string key)
        {
            return m_dicoRsaConnectionState[key];
        }

        public void Remove(string key)
        {
            m_dicoRsaConnectionState.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return m_dicoRsaConnectionState.ContainsKey(key);
        }

        public bool ContainsValue(RsaConnectionState rsaConnectionState)
        {
            return m_dicoRsaConnectionState.ContainsValue(rsaConnectionState);
        }

        public int Count()
        {
            return m_dicoRsaConnectionState.Count;
        }

        public void Clear()
        {
            m_dicoRsaConnectionState.Clear();
        }

        public Dictionary<string, RsaConnectionState>.KeyCollection GetKeys()
        {
            return m_dicoRsaConnectionState.Keys;
        }

        public Dictionary<string, RsaConnectionState>.ValueCollection GetValues()
        {
            return m_dicoRsaConnectionState.Values;
        }

    }
}
