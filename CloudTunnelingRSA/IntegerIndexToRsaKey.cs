using CloudTunnelingRSA.Beans;
using CloudTunnelingRSA.Dico;

namespace CloudTunnelingRSA
{
    public class IntegerIndexToRsaKey
    {
        public int m_index = int.MinValue;
        public RsaPublicKeyRef m_rsaKey;
        public void Set(int index, RsaPublicKeyRef rsaKey)
        {
            m_index = index;
            m_rsaKey = rsaKey;
        }
        public void Set(int index, string rsaKey)
        {
            m_index = index;
            m_rsaKey = DicoRefRsaPublicKey.Instance.GetOrCreate(rsaKey);
        }
    }
}
