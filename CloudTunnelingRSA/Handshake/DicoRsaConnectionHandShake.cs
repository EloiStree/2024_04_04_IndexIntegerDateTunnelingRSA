using CloudTunnelingRSA.Beans;

public class DicoRsaConnectionHandShake
    {

        public static DicoRsaConnectionHandShake Instance = new DicoRsaConnectionHandShake();
        public Dictionary<ulong, RsaConnectionHandShake> m_dicoIntegerIndexToRsaConnectionHandShake = new Dictionary<ulong, RsaConnectionHandShake>();

    public RsaConnectionHandShake CreateOrSetThenGet(RsaPublicKeyRef key)
    {
        ulong i = key.GetObjectIntId();
        if (m_dicoIntegerIndexToRsaConnectionHandShake.ContainsKey(i))
        {
            return m_dicoIntegerIndexToRsaConnectionHandShake[i];
        }
        else
        {
            m_dicoIntegerIndexToRsaConnectionHandShake.Add(i, new RsaConnectionHandShake(key));
            return m_dicoIntegerIndexToRsaConnectionHandShake[i];
        }
    }
    public RsaConnectionHandShake GetIfExistsOrNull(RsaPublicKeyRef key)
    {
        ulong i = key.GetObjectIntId();
        if (m_dicoIntegerIndexToRsaConnectionHandShake.ContainsKey(i))
        {
            return m_dicoIntegerIndexToRsaConnectionHandShake[i];
        }
        return null;
    }
}
