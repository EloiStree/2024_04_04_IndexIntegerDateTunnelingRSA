using CloudTunnelingRSA.Beans;

public class DicoRsaConnectionHandShake
    {

        public static DicoRsaConnectionHandShake Instance = new DicoRsaConnectionHandShake();
        public Dictionary<ulong,List< RsaConnectionHandShake>> m_dicoIntegerIndexToRsaConnectionHandShake = new Dictionary<ulong,List< RsaConnectionHandShake>>();

    public void AddHandshake(RsaPublicKeyRef key, in RsaConnectionHandShake handshake)
    {

        ulong i = key.GetObjectMemoryId();
        if (m_dicoIntegerIndexToRsaConnectionHandShake.ContainsKey(i))
        {
            m_dicoIntegerIndexToRsaConnectionHandShake[i].Add(handshake);
        }
        else
        {
            m_dicoIntegerIndexToRsaConnectionHandShake.Add(i, new List<RsaConnectionHandShake>());
            m_dicoIntegerIndexToRsaConnectionHandShake[i].Add(handshake);
        }
    }
    public void RemoveHandshake(RsaPublicKeyRef key, in RsaConnectionHandShake handshake)
    {

        ulong i = key.GetObjectMemoryId();
        if ( m_dicoIntegerIndexToRsaConnectionHandShake.ContainsKey(i) )
        {
            List<RsaConnectionHandShake> list = m_dicoIntegerIndexToRsaConnectionHandShake[i];
            for (int j = list.Count - 1; j >= 0; j--)
            {
                if (list[j] == handshake)
                {
                    list.RemoveAt(j);
                }
            }
        }
        else
        {
            m_dicoIntegerIndexToRsaConnectionHandShake.Add(i, new List<RsaConnectionHandShake>());
        }
    }

    public List<ulong> GetKeys()
    {
        return m_dicoIntegerIndexToRsaConnectionHandShake.Keys.ToList();
    }

    public List<RsaConnectionHandShake> GetIfExistsOrNull(RsaPublicKeyRef key)
    {
        ulong i = key.GetObjectMemoryId();
        if (m_dicoIntegerIndexToRsaConnectionHandShake.ContainsKey(i))
        {
            return m_dicoIntegerIndexToRsaConnectionHandShake[i];
        }
        return null;
    }
    public List<RsaConnectionHandShake> GetIfExistsOrNull(ulong key)
    {
        if (m_dicoIntegerIndexToRsaConnectionHandShake.ContainsKey(key))
        {
            return m_dicoIntegerIndexToRsaConnectionHandShake[key];
        }
        return null;
    }

    public void Remove(RsaConnectionHandShake handshake)
    {
        if(handshake==null)
            return;
        ulong i = handshake.GetPublicKey().GetObjectMemoryId();
        if (m_dicoIntegerIndexToRsaConnectionHandShake.ContainsKey(i))
        {
            List<RsaConnectionHandShake> list = m_dicoIntegerIndexToRsaConnectionHandShake[i];
            for (int j = list.Count - 1; j >= 0; j--)
            {
                if (list[j] == handshake)
                {
                    list.RemoveAt(j);
                }
            }
        }
    }
}
