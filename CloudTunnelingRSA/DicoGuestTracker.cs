using CloudTunnelingRSA.Beans;

public class DicoGuestTracker
{
    public static DicoGuestTracker Instance = new DicoGuestTracker();

    public Dictionary<ulong, RsaPublicKeyRef> m_rsaIsGuest = new Dictionary<ulong, RsaPublicKeyRef>();
    public Dictionary<int, ulong> m_indexToRSAIsGuest =new Dictionary<int, ulong>();
    public Dictionary<ulong, int> m_rsaToIndexIsGuest=new Dictionary<ulong, int>();

    public void Add(int index, RsaPublicKeyRef publicKey)
    {
        if(publicKey==null)
            return;

        if (m_indexToRSAIsGuest.ContainsKey(index))
            return;
        
        ulong id = publicKey.GetObjectMemoryId(); 
        if (m_rsaToIndexIsGuest.ContainsKey(id))
            return;

        m_rsaIsGuest.Add(id, publicKey);
        m_indexToRSAIsGuest.Add(index, id);
        m_rsaToIndexIsGuest.Add(id, index);
    }

    public void RemoveFromIndex(int index)
    {
           //remove from index
           if(m_indexToRSAIsGuest.ContainsKey(index))
        {
               ulong id = m_indexToRSAIsGuest[index];
               m_indexToRSAIsGuest.Remove(index);
               m_rsaIsGuest.Remove(id);
               m_rsaToIndexIsGuest.Remove(id);
           }
    }
    public void RemoveFromKey(RsaPublicKeyRef key)
    {
        if(key==null)
            return;
        RemoveFromKey(key.GetObjectMemoryId());
    }
    public void RemoveFromKey(ulong key)
    {
        if(m_rsaIsGuest.ContainsKey(key))
        {
            int index = m_rsaToIndexIsGuest[key];
            m_rsaIsGuest.Remove(key);
            m_indexToRSAIsGuest.Remove(index);
            m_rsaToIndexIsGuest.Remove(key);
        }
    }

    public int GetValuesCount()
    {
        return m_rsaIsGuest.Count();
    }
}